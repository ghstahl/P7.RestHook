using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using P7.RestHook.ClientManagement;
using P7.RestHook.ClientManagement.Models;
using P7.RestHook.Models;
using P7.RestHook.Store;
using Shouldly;

namespace UnitTest.RestHookStore.Core.Stores
{
    public abstract class UnitTestRestHookClientStore
    {
        private IRestHookClientManagementStore _restHookClientManagementStore;
        private IRestHookClientManagementStoreTest _restHookClientManagementStoreTest;

        private static HookUserWithClients UniqueHookUserWithClientsClientsRecord => new HookUserWithClients
        {
            UserId = Unique.S,
           Clients = new List<HookClientWithHookRecords>()
        };

        public UnitTestRestHookClientStore(
            IRestHookClientManagementStoreTest restHookClientManagementStoreTest,
            IRestHookClientManagementStore restHookClientManagementStore)
        {
            _restHookClientManagementStoreTest = restHookClientManagementStoreTest;
            _restHookClientManagementStore = restHookClientManagementStore;
        }

        [TestMethod]
        public async Task DI_Valid()
        {
            _restHookClientManagementStoreTest.ShouldNotBeNull();
            _restHookClientManagementStore.ShouldNotBeNull();
        }

        [TestInitialize]
        public async Task Initialize()
        {
            await _restHookClientManagementStoreTest.DropAsync();
        }
        [TestMethod]
        public async Task Find_nonexistant_user()
        {
            var result = await _restHookClientManagementStore.FindHookUserAsync(Unique.S);
            result.ShouldNotBeNull();
            result.Success.ShouldBeFalse();
            result.Error.ShouldNotBeNull();
        }
        [TestMethod]
        public async Task Create_client_delete_user_fail()
        {
            var userId = Unique.S;

            var result = await _restHookClientManagementStore.UpsertHookUserAsync(userId);
            result.ShouldNotBeNull();
            result.Data.ShouldNotBeNull();

            var clientRecordResult = await _restHookClientManagementStore.CreateHookClientAsync(userId);
            clientRecordResult.ShouldNotBeNull();
            clientRecordResult.Success.ShouldBeTrue();
            var clientRecord = clientRecordResult.Data;

            var clientResult = await _restHookClientManagementStore
                .FindHookClientAsync(userId, clientRecord.ClientId);
            clientResult.ShouldNotBeNull();
            clientResult.Success.ShouldBeTrue();
            clientResult.Data.ShouldNotBeNull();
            clientResult.Data.ClientId.ShouldBe(clientRecord.ClientId);

            var rr = await _restHookClientManagementStore.DeleteHookUserAsync(userId);
            rr.Success.ShouldBeFalse();
        }
        [TestMethod]
        public async Task Create_client_delete()
        {
            var userId = Unique.S;
 
            var result = await _restHookClientManagementStore.UpsertHookUserAsync(userId);
            result.ShouldNotBeNull();
            result.Data.ShouldNotBeNull();

            var clientRecordResult = await _restHookClientManagementStore.CreateHookClientAsync(userId);
            clientRecordResult.ShouldNotBeNull();
            clientRecordResult.Success.ShouldBeTrue();
            var clientRecord = clientRecordResult.Data;

            clientRecordResult = await _restHookClientManagementStore.CreateHookClientAsync(userId);
            clientRecordResult.ShouldNotBeNull();
            clientRecordResult.Success.ShouldBeTrue();
            clientRecord = clientRecordResult.Data;

            var clientResult = await _restHookClientManagementStore
                .FindHookClientAsync(userId, clientRecord.ClientId);
            clientResult.ShouldNotBeNull();
            clientResult.Success.ShouldBeTrue();
            clientResult.Data.ShouldNotBeNull();
            clientResult.Data.ClientId.ShouldBe(clientRecord.ClientId);


            var rr = await _restHookClientManagementStore.DeleteHookClientAsync(userId,clientRecord.ClientId);
            rr.Success.ShouldBeTrue();

            clientResult = await _restHookClientManagementStore
                .FindHookClientAsync(userId, clientRecord.ClientId);
            clientResult.ShouldNotBeNull();
            clientResult.Success.ShouldBeFalse();
           

        }

        class UserClientsRecord
        {
            public string UserId { get; set; }
            public List<HookClient> HookClients { get; set; }
        }
        async Task<(
            List<UserClientsRecord> producerRecords,
            List<UserClientsRecord> consumerRecords,
            List<string> eventNames)>
             CreateFullDataSuiteAsync()
        {
            var producerRecords = new List<UserClientsRecord>()
            {
                new UserClientsRecord(){UserId = Unique.S,HookClients = new List<HookClient>()},
                new UserClientsRecord(){UserId = Unique.S,HookClients = new List<HookClient>()}
            };
            var consumerRecords = new List<UserClientsRecord>()
            {
                new UserClientsRecord(){UserId = Unique.S,HookClients = new List<HookClient>()},
                new UserClientsRecord(){UserId = Unique.S,HookClients = new List<HookClient>()}
            };

            List<string> eventNames = new List<string>();
            for (var i = 0; i < 2; ++i)
            {
                eventNames.Add(Unique.S);
            }

            // create Producer Side
            // 2 producer users, each with 2 clients and 2 events
            foreach (var producerRecord in producerRecords)
            {
                var userId = producerRecord.UserId;
                var result = await _restHookClientManagementStore.UpsertHookUserAsync(userId);
                result.ShouldNotBeNull();
                result.Data.ShouldNotBeNull();
                for (int iClient = 0; iClient < 2; ++iClient)
                {
                    var clientRecordResult = await _restHookClientManagementStore
                        .CreateHookClientAsync(userId);
                    clientRecordResult.ShouldNotBeNull();
                    clientRecordResult.Success.ShouldBeTrue();
                    var hookClient = clientRecordResult.Data;
                    producerRecord.HookClients.Add(hookClient);
                    foreach (var eventName in eventNames)
                    {
                        var eventCreateResult =
                            await _restHookClientManagementStore.AddProducesHookEventAsync(
                                userId, hookClient.ClientId, eventName);
                        eventCreateResult.ShouldNotBeNull();
                        eventCreateResult.Success.ShouldBeTrue();

                        var eventResult = await _restHookClientManagementStore
                            .FindHookEventAsync(userId, hookClient.ClientId, eventName);
                        eventResult.ShouldNotBeNull();
                        eventResult.Success.ShouldBeTrue();
                        eventResult.Data.ShouldNotBeNull();
                        var hookEvent = eventResult.Data;
                        hookEvent.Name.ShouldBe(eventName);
                    }
                }
            }
            // create Consumer Side
            foreach (var consumerRecord in consumerRecords)
            {
                var userId = consumerRecord.UserId;
                var result = await _restHookClientManagementStore.UpsertHookUserAsync(userId);
                result.ShouldNotBeNull();
                result.Data.ShouldNotBeNull();
                for (int iClient = 0; iClient < 2; ++iClient)
                {
                    var clientRecordResult = await _restHookClientManagementStore
                        .CreateHookClientAsync(userId);
                    clientRecordResult.ShouldNotBeNull();
                    clientRecordResult.Success.ShouldBeTrue();
                    var hookClient = clientRecordResult.Data;
                    consumerRecord.HookClients.Add(hookClient);
                }
            }

            // hook them up.
            foreach (var producerRecord in producerRecords)
            {
                foreach (var producerHookClient in producerRecord.HookClients)
                {
                    foreach (var consumerRecord in consumerRecords)
                    {
                        foreach (var consumerHookClient in consumerRecord.HookClients)
                        {
                            foreach (var eventName in eventNames)
                            {
                                var consumesEventResult = await _restHookClientManagementStore
                                    .AddConsumerHookEventAsync(
                                        producerRecord.UserId, producerHookClient.ClientId, eventName,
                                        consumerRecord.UserId, consumerHookClient.ClientId, Unique.Url);

                                consumesEventResult.ShouldNotBeNull();
                                consumesEventResult.Success.ShouldBeTrue();

                                var callbackResults = await _restHookClientManagementStore
                                    .FindProducerHookEventCallbackUrlsAsync(
                                        producerRecord.UserId,
                                        producerHookClient.ClientId, eventName);

                                callbackResults.ShouldNotBeNull();
                                callbackResults.Success.ShouldBeTrue();
                                callbackResults.Data.ShouldNotBeNull();
                            }
                        }
                    }
                }
            }
            return (producerRecords, consumerRecords, eventNames);
        }
        [TestMethod]
        public async Task DeepClean_Consumer_client()
        {
            var (producerRecords, consumerRecords, eventNames) = await CreateFullDataSuiteAsync();

            var findEventResult =
                await _restHookClientManagementStore.FindProducerHookEventCallbackUrlsAsync(
                    consumerRecords[0].UserId, consumerRecords[0].HookClients[0].ClientId, eventNames[0]);
            findEventResult.ShouldNotBeNull();
            findEventResult.Success.ShouldBeTrue();
            findEventResult.Data.Count().ShouldBe(2);

            findEventResult =
                await _restHookClientManagementStore.FindProducerHookEventCallbackUrlsAsync(
                    consumerRecords[0].UserId, consumerRecords[0].HookClients[1].ClientId, eventNames[0]);

            findEventResult.ShouldNotBeNull();
            findEventResult.Success.ShouldBeTrue();
            findEventResult.Data.Count().ShouldBe(2);

            var deepCleanResult =
                await _restHookClientManagementStore
                    .DeepCleanConsumerHookClientAsync(consumerRecords[0].UserId, consumerRecords[0].HookClients[0].ClientId);
            deepCleanResult.ShouldNotBeNull();
            deepCleanResult.Success.ShouldBeTrue();


            findEventResult =
                await _restHookClientManagementStore.FindProducerHookEventCallbackUrlsAsync(
                    consumerRecords[0].UserId, consumerRecords[0].HookClients[0].ClientId, eventNames[0]);
            findEventResult.ShouldNotBeNull();
            findEventResult.Success.ShouldBeTrue();
            findEventResult.Data.Count().ShouldBe(0);

            findEventResult =
                await _restHookClientManagementStore.FindProducerHookEventCallbackUrlsAsync(
                    consumerRecords[0].UserId, consumerRecords[0].HookClients[1].ClientId, eventNames[0]);

            findEventResult.ShouldNotBeNull();
            findEventResult.Success.ShouldBeTrue();
            findEventResult.Data.Count().ShouldBe(2);

        }
        [TestMethod]
        public async Task DeepClean_Producer_client()
        {
           
            var consumerUserId = Unique.S;

            List<string> UserIds = new List<string>();
            List<HookClient> producerClients = new List<HookClient>();
            List<string> eventNames = new List<string>();
            for (int i = 0; i < 10; ++i)
            {
                eventNames.Add(Unique.S);
            }

            for (int i = 0; i < 2; ++i)
            {
                var userId = Unique.S;
                UserIds.Add(userId);
                var result = await _restHookClientManagementStore.UpsertHookUserAsync(userId);
                result.ShouldNotBeNull();
                result.Data.ShouldNotBeNull();

                var clientRecordResult = await _restHookClientManagementStore
                    .CreateHookClientAsync(userId);
                clientRecordResult.ShouldNotBeNull();
                clientRecordResult.Success.ShouldBeTrue();
                var hookClient = clientRecordResult.Data;
                producerClients.Add(hookClient);

               
                for (int iEvent = 0; iEvent < 10; ++iEvent)
                {
                    var eventName = eventNames[iEvent];
                    var eventCreateResult =
                        await _restHookClientManagementStore.AddProducesHookEventAsync(
                            userId, hookClient.ClientId, eventName);
                    eventCreateResult.ShouldNotBeNull();
                    eventCreateResult.Success.ShouldBeTrue();

                    var eventResult = await _restHookClientManagementStore
                        .FindHookEventAsync(userId, hookClient.ClientId, eventName);
                    eventResult.ShouldNotBeNull();
                    eventResult.Success.ShouldBeTrue();
                    eventResult.Data.ShouldNotBeNull();
                    var hookEvent = eventResult.Data;
                    hookEvent.Name.ShouldBe(eventName);
                }
            }


            var result2 = await _restHookClientManagementStore.UpsertHookUserAsync(consumerUserId);
            result2.ShouldNotBeNull();
            result2.Data.ShouldNotBeNull();

            var clientRecordResult2 = await _restHookClientManagementStore
                .CreateHookClientAsync(consumerUserId);
            clientRecordResult2.ShouldNotBeNull();
            clientRecordResult2.Success.ShouldBeTrue();
            var consumerClientRecord = clientRecordResult2.Data;

            for (int i = 0; i < 2; ++i)
            {
                var userId = UserIds[i];
                var producerClient = producerClients[i];
                for (int iEvent = 0; iEvent < 10; ++iEvent)
                {
                    var eventName = eventNames[iEvent];

                    var consumesEventResult = await _restHookClientManagementStore
                        .AddConsumerHookEventAsync(
                            userId, producerClient.ClientId, eventName,
                            consumerUserId, consumerClientRecord.ClientId, Unique.Url);

                    consumesEventResult.ShouldNotBeNull();
                    consumesEventResult.Success.ShouldBeTrue();

                    var callbackResults = await _restHookClientManagementStore
                        .FindProducerHookEventCallbackUrlsAsync(
                            userId,
                            producerClient.ClientId, eventName);

                    callbackResults.ShouldNotBeNull();
                    callbackResults.Success.ShouldBeTrue();
                    callbackResults.Data.ShouldNotBeNull();
                    callbackResults.Data.Count().ShouldBe(1);
                }
            }

         
          

            var deepCleanResult =
                await _restHookClientManagementStore
                    .DeepCleanProducerHookClientAsync(UserIds[0], producerClients[0].ClientId);
            deepCleanResult.ShouldNotBeNull();
            deepCleanResult.Success.ShouldBeTrue();

            var findEventResult =
                await _restHookClientManagementStore.FindHookEventsAsync(UserIds[0], producerClients[0].ClientId);
            findEventResult.ShouldNotBeNull();
            findEventResult.Success.ShouldBeTrue();
            findEventResult.Data.Count().ShouldBe(0);

            findEventResult =
                await _restHookClientManagementStore.FindHookEventsAsync(UserIds[1], producerClients[1].ClientId);
            findEventResult.ShouldNotBeNull();
            findEventResult.Success.ShouldBeTrue();
            findEventResult.Data.Count().ShouldBe(10);

            /*
                        var deleteResult = await _restHookClientManagementStore.DeleteConsumerHookEventAsync(
                            consumerUserId, consumerClientRecord.ClientId, eventName);
                        deleteResult.ShouldNotBeNull();
                        deleteResult.Success.ShouldBeTrue();

                        callbackResults = await _restHookClientManagementStore.FindProducerHookEventCallbackUrlsAsync(userId,
                            clientRecord.ClientId, eventName);

                        callbackResults.ShouldNotBeNull();
                        callbackResults.Success.ShouldBeTrue();
                        callbackResults.Data.ShouldNotBeNull();
                        callbackResults.Data.Count().ShouldBe(0);
            */
        }
        [TestMethod]
        public async Task Create_consumes_event_delete()
        {
            var userId = Unique.S;
            var consumerUserId = Unique.S;

            var result = await _restHookClientManagementStore.UpsertHookUserAsync(userId);
            result.ShouldNotBeNull();
            result.Data.ShouldNotBeNull();

            result = await _restHookClientManagementStore.UpsertHookUserAsync(consumerUserId);
            result.ShouldNotBeNull();
            result.Data.ShouldNotBeNull();

            var clientRecordResult = await _restHookClientManagementStore.CreateHookClientAsync(userId);
            clientRecordResult.ShouldNotBeNull();
            clientRecordResult.Success.ShouldBeTrue();
            var clientRecord = clientRecordResult.Data;

            clientRecordResult = await _restHookClientManagementStore.CreateHookClientAsync(consumerUserId);
            clientRecordResult.ShouldNotBeNull();
            clientRecordResult.Success.ShouldBeTrue();
            var consumerClientRecord = clientRecordResult.Data;

            var eventName = "test_event";
            var eventCreateResult =
                await _restHookClientManagementStore.AddProducesHookEventAsync(userId,
                    clientRecord.ClientId, eventName);
            eventCreateResult.ShouldNotBeNull();
            eventCreateResult.Success.ShouldBeTrue();

            var eventResult = await _restHookClientManagementStore
                .FindHookEventAsync(userId, clientRecord.ClientId, eventName);
            eventResult.ShouldNotBeNull();
            eventResult.Success.ShouldBeTrue();
            eventResult.Data.ShouldNotBeNull();
            var hookEvent = eventResult.Data;
            hookEvent.Name.ShouldBe(eventName);

            var consumesEventResult = await _restHookClientManagementStore.AddConsumerHookEventAsync(userId,
                clientRecord.ClientId, eventName, consumerUserId, consumerClientRecord.ClientId,Unique.Url);

            consumesEventResult.ShouldNotBeNull();
            consumesEventResult.Success.ShouldBeTrue();

            var callbackResults = await _restHookClientManagementStore.FindProducerHookEventCallbackUrlsAsync(userId,
                clientRecord.ClientId, eventName);

            callbackResults.ShouldNotBeNull();
            callbackResults.Success.ShouldBeTrue();
            callbackResults.Data.ShouldNotBeNull();
            callbackResults.Data.Count().ShouldBe(1);

            var deleteResult = await _restHookClientManagementStore.DeleteConsumerHookEventAsync(
                consumerUserId, consumerClientRecord.ClientId, eventName);
            deleteResult.ShouldNotBeNull();
            deleteResult.Success.ShouldBeTrue();

            callbackResults = await _restHookClientManagementStore.FindProducerHookEventCallbackUrlsAsync(userId,
                clientRecord.ClientId, eventName);

            callbackResults.ShouldNotBeNull();
            callbackResults.Success.ShouldBeTrue();
            callbackResults.Data.ShouldNotBeNull();
            callbackResults.Data.Count().ShouldBe(0);

        }
        [TestMethod]
        public async Task Create_event_delete()
        {
            var userId = Unique.S;

            var result = await _restHookClientManagementStore.UpsertHookUserAsync(userId);
            result.ShouldNotBeNull();
            result.Data.ShouldNotBeNull();

            var clientRecordResult = await _restHookClientManagementStore.CreateHookClientAsync(userId);
            clientRecordResult.ShouldNotBeNull();
            clientRecordResult.Success.ShouldBeTrue();
            var clientRecord = clientRecordResult.Data;

            var eventName = "test_event";
            var eventCreateResult =
                await _restHookClientManagementStore.AddProducesHookEventAsync(userId, 
                    clientRecord.ClientId, eventName);
            eventCreateResult.ShouldNotBeNull();
            eventCreateResult.Success.ShouldBeTrue();

            var eventResult = await _restHookClientManagementStore
                .FindHookEventAsync(userId, clientRecord.ClientId, eventName);
            eventResult.ShouldNotBeNull();
            eventResult.Success.ShouldBeTrue();
            eventResult.Data.ShouldNotBeNull();
            var hookEvent = eventResult.Data;
           hookEvent.Name.ShouldBe(eventName);

            var deleteResult = await _restHookClientManagementStore.DeleteHookEventAsync(userId, clientRecord.ClientId,
                eventName);
            deleteResult.ShouldNotBeNull();
            deleteResult.Success.ShouldBeTrue();

            eventResult = await _restHookClientManagementStore
                .FindHookEventAsync(userId, clientRecord.ClientId, eventName);
            eventResult.ShouldNotBeNull();
            eventResult.Success.ShouldBeFalse();

        }
        [TestMethod]
        public async Task Find_multiple_events()
        {
            var userId = Unique.S;

            var result = await _restHookClientManagementStore.UpsertHookUserAsync(userId);
            result.ShouldNotBeNull();
            result.Data.ShouldNotBeNull();

            var clientRecordResult = await _restHookClientManagementStore.CreateHookClientAsync(userId);
            clientRecordResult.ShouldNotBeNull();
            clientRecordResult.Success.ShouldBeTrue();
            var clientRecord = clientRecordResult.Data;

            var itemCount = 10;
            for (var i=0;i< itemCount; ++i)
            {
                var eventCreateResult =
                    await _restHookClientManagementStore.AddProducesHookEventAsync(userId,
                        clientRecord.ClientId, Unique.S);
                eventCreateResult.ShouldNotBeNull();
                eventCreateResult.Success.ShouldBeTrue();
            }
           

            var eventResult = await _restHookClientManagementStore
                .FindHookEventsAsync(userId, clientRecord.ClientId);
            eventResult.ShouldNotBeNull();
            eventResult.Success.ShouldBeTrue();
            eventResult.Data.ShouldNotBeNull();
            var hookEvents = eventResult.Data;
            var count = hookEvents.Count();
            count.ShouldBe(itemCount);
           

        }
        [TestMethod]
        public async Task HookUser_Create_Delete()
        {
            var userId = Unique.S;
            var result = await _restHookClientManagementStore.FindHookUserAsync(userId);
            result.ShouldNotBeNull();
            result.Data.ShouldBeNull();

            result = await _restHookClientManagementStore.UpsertHookUserAsync(userId);
            result.ShouldNotBeNull();
            result.Data.ShouldNotBeNull();

            var clientRecordResult = await _restHookClientManagementStore.CreateHookClientAsync(userId);
            clientRecordResult.ShouldNotBeNull();
            clientRecordResult.Success.ShouldBeTrue();
            var clientRecord = clientRecordResult.Data;

            var result2 = await _restHookClientManagementStore.DeleteHookUserAsync(userId);
            result2.ShouldNotBeNull();
            result2.Success.ShouldBeTrue();

            result = await _restHookClientManagementStore.FindHookUserAsync(userId);
            result.ShouldNotBeNull();
            result.Data.ShouldBeNull();
            result.Success.ShouldBeFalse();
            result.Error.ShouldNotBeNull();
        }

        [TestMethod]
        public async Task ClientStore_Create_hookrecord_add_delete()
        {

            var userId = Unique.S;
            var result = await _restHookClientManagementStore.UpsertHookUserAsync(userId);
            result.ShouldNotBeNull();
            result.Data.ShouldNotBeNull();

            var hookUserClientsRecord = result.Data;
            var clientRecordResult = await _restHookClientManagementStore.CreateHookClientAsync(userId);
            clientRecordResult.ShouldNotBeNull();
            clientRecordResult.Success.ShouldBeTrue();
            clientRecordResult.Data.ShouldNotBeNull();

            var clientRecord = clientRecordResult.Data;
            var hookRecord = new HookRecord()
            {
                ClientId = clientRecord.ClientId,
                CallbackUrl = "https://www.somedomain.com",
                EventName = Unique.S
            };

            var hookRecordResult = await _restHookClientManagementStore.AddHookRecordAsync(userId, hookRecord);
            hookRecordResult.ShouldNotBeNull();
            hookRecordResult.Success.ShouldBeTrue();
            hookRecordResult.Data.ShouldNotBeNull();

            var hookRecordsResult =
                await _restHookClientManagementStore.FindHookRecordsAsync(userId, clientRecord.ClientId);
            hookRecordsResult.ShouldNotBeNull();
            hookRecordsResult.Success.ShouldBeTrue();
            hookRecordsResult.Data.ShouldNotBeNull();
            hookRecordsResult.Data.Count().ShouldBe(1);

            var deleteHookRecordResult =
                await _restHookClientManagementStore.DeleteHookRecordAsync(userId, clientRecord.ClientId, hookRecordResult.Data.Id);
            deleteHookRecordResult.ShouldNotBeNull();
            deleteHookRecordResult.Success.ShouldBeTrue();
            

            hookRecordsResult =
                await _restHookClientManagementStore.FindHookRecordsAsync(userId, clientRecord.ClientId);
            hookRecordsResult.ShouldNotBeNull();
            hookRecordsResult.Success.ShouldBeTrue();
            hookRecordsResult.Data.ShouldNotBeNull();
            hookRecordsResult.Data.Count().ShouldBe(0);

        }

        [TestMethod]
        public async Task ClientStore_Create_user_client_hookrecord_Delete()
        {
            

            var userId = Unique.S;
            var result = await _restHookClientManagementStore.FindHookUserAsync(userId);
            result.ShouldNotBeNull();
            result.Data.ShouldBeNull();

            result = await _restHookClientManagementStore.UpsertHookUserAsync(userId);
            result.ShouldNotBeNull();
            result.Data.ShouldNotBeNull();

            var hookUserClientsRecord = result.Data;
            var clientRecordResult = await _restHookClientManagementStore.CreateHookClientAsync(userId);
            clientRecordResult.ShouldNotBeNull();
            clientRecordResult.Success.ShouldBeTrue();
            clientRecordResult.Data.ShouldNotBeNull();

            var clientRecord = clientRecordResult.Data;
            var hookRecord = new HookRecord()
            {
                ClientId = clientRecord.ClientId,
                CallbackUrl = "https://www.somedomain.com",
                EventName = Unique.S
            };

            var hookRecordResult = await _restHookClientManagementStore.AddHookRecordAsync(userId, hookRecord);
            hookRecordResult.ShouldNotBeNull();
            hookRecordResult.Success.ShouldBeTrue();
            hookRecordResult.Data.ShouldNotBeNull();

            var finalHookRecord = hookRecordResult.Data;
            finalHookRecord.ClientId.ShouldBe(hookRecord.ClientId);
            finalHookRecord.CallbackUrl.ShouldBe(hookRecord.CallbackUrl);
            finalHookRecord.EventName.ShouldBe(hookRecord.EventName);

            var rrss = await _restHookClientManagementStore.FindHookUserClientsAsync(userId);
            rrss.ShouldNotBeNull();
            rrss.Data.ShouldNotBeNull();
            var hookClients = rrss.Data;
            hookClients.Count.ShouldBe(1);
                
 
            var result2 = await _restHookClientManagementStore.DeleteHookUserAsync(userId);
            result2.ShouldNotBeNull();
            result2.Success.ShouldBeTrue();

            result = await _restHookClientManagementStore.FindHookUserAsync(userId);
            result.ShouldNotBeNull();
            result.Data.ShouldBeNull();
        }

        [TestMethod]
        public async Task ClientStore_find_null_user_throws()
        {
            string userId = null;
            await Should.ThrowAsync<Exception>(
                async () => await
                    _restHookClientManagementStore.FindHookUserAsync(userId)
            );

        }

        [TestMethod]
        public async Task Delete_nonexistant_user()
        {
            var userId = Unique.S;

            var result = await _restHookClientManagementStore.DeleteHookUserAsync(userId);
            result.ShouldNotBeNull();
            result.Success.ShouldBeTrue();
        }
        [TestMethod]
        public async Task Create_user_delete()
        {
            var userId = Unique.S;

            var result = await _restHookClientManagementStore.UpsertHookUserAsync(userId);
            result.ShouldNotBeNull();
            result.Data.ShouldNotBeNull();

            var rr = await _restHookClientManagementStore.FindHookUserAsync(userId);
            rr.Success.ShouldBeTrue();

            var delUserResult = await _restHookClientManagementStore.DeleteHookUserAsync(userId);
            delUserResult.ShouldNotBeNull();
            delUserResult.Success.ShouldBeTrue();

            rr = await _restHookClientManagementStore.FindHookUserAsync(userId);
            rr.Success.ShouldBeFalse();

        }
        [TestMethod]
        public async Task Delete_nonexistant_client()
        {          
            var userId = Unique.S;

            var result = await _restHookClientManagementStore.UpsertHookUserAsync(userId);
            result.ShouldNotBeNull();
            result.Data.ShouldNotBeNull();

            var createClientResult = await _restHookClientManagementStore.DeleteHookClientAsync(userId,Unique.G);
            createClientResult.ShouldNotBeNull();
            createClientResult.Success.ShouldBeTrue();

        }
        [TestMethod]
        public async Task Create_client_on_nonexistant_user()
        {
            var userId = Unique.S;
            var createClientResult = await _restHookClientManagementStore.CreateHookClientAsync(userId);
            createClientResult.ShouldNotBeNull();
            createClientResult.Success.ShouldBeFalse();
            createClientResult.Error.ShouldNotBeNull();
 
        }
        [TestMethod]
        public async Task ClientStore_create_user_add_client_success()
        {
            var userId = Unique.S;
            // create recrod
            var record = await _restHookClientManagementStore.UpsertHookUserAsync(userId);
            record.ShouldNotBeNull();

            // Create Hook ClientWithHookRecords
            var clientRecordResult = await _restHookClientManagementStore.CreateHookClientAsync(record.Data.UserId);
            clientRecordResult.ShouldNotBeNull();
            clientRecordResult.Success.ShouldBeTrue();

            var clientRecord = clientRecordResult.Data;


            var rrss = await _restHookClientManagementStore.FindHookUserClientsAsync(userId);
            rrss.ShouldNotBeNull();
            rrss.Data.ShouldNotBeNull();
            var hookClients = rrss.Data;
            hookClients.Count.ShouldBe(1);

        }

        [TestMethod]
        public async Task ClientStore_add_client_delete()
        {
            var userId = Unique.S;
            // create record
            var record = await _restHookClientManagementStore.UpsertHookUserAsync(userId);
            record.ShouldNotBeNull();


            // still try to use it and update it
            var clientRecordResult = await _restHookClientManagementStore.CreateHookClientAsync(record.Data.UserId);
            clientRecordResult.ShouldNotBeNull();
            clientRecordResult.Success.ShouldBeTrue();
            var clientRecord = clientRecordResult.Data;


            var rrss = await _restHookClientManagementStore.FindHookUserClientsAsync(userId);
            rrss.ShouldNotBeNull();
            rrss.Data.ShouldNotBeNull();
            var hookClients = rrss.Data;


            hookClients.Count.ShouldBe(1);

            false.ShouldBe(true);// still have work to do

        }

        [TestMethod]
        public async Task Find_nonexistant_event()
        {
            var userId = Unique.S;

            var result2 = await _restHookClientManagementStore.FindHookEventAsync(userId, Unique.S, Unique.S);
            result2.ShouldNotBeNull();
            result2.Success.ShouldBeFalse();
            result2.Error.ShouldNotBeNull();
        }
        [TestMethod]
        public async Task Find_nonexistant_client()
        {
            var userId = Unique.S;

            var result2 = await _restHookClientManagementStore
                .FindHookClientAsync(userId, Unique.S);
            result2.ShouldNotBeNull();
            result2.Success.ShouldBeFalse();
            result2.Error.ShouldNotBeNull();
        }
    }

    class ClientRecordEqualityCompare : IEqualityComparer<HookClientWithHookRecords>
    {
        public bool Equals(HookClientWithHookRecords x, HookClientWithHookRecords y)
        {
            return x.ClientId == y.ClientId;
        }

        public int GetHashCode(HookClientWithHookRecords obj)
        {
            return obj.ClientId.GetHashCode();
        }
    }
}