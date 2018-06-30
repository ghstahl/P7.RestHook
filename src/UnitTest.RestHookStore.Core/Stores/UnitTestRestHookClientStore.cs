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

        private static HookUserClientsRecord UniqueHookUserClientsRecord => new HookUserClientsRecord
        {
            UserId = Unique.S,
           Clients = new List<ClientRecord>()
        };

        public UnitTestRestHookClientStore(IRestHookClientManagementStore restHookClientManagementStore)
        {
            _restHookClientManagementStore = restHookClientManagementStore;
        }

        [TestMethod]
        public async Task DI_Valid()
        {
            _restHookClientManagementStore.ShouldNotBeNull();
        }
        [TestMethod]
        public async Task ClientStore_Not_Found()
        {
            var result = await _restHookClientManagementStore.FindHookUserClientsAsync(Unique.S);
            result.ShouldNotBeNull();
            result.Success.ShouldBeTrue();
            result.Data.ShouldBeNull();
        }
        [TestMethod]
        public async Task ClientStore_Not_Found_Create()
        {
            var userId = Unique.S;
            var result = await _restHookClientManagementStore.FindHookUserClientsAsync(userId);
            result.ShouldNotBeNull();
            result.Success.ShouldBeTrue();
            result.Data.ShouldBeNull();

            result = await _restHookClientManagementStore.CreateHookUserClientAsync(userId);
            result.ShouldNotBeNull();
            result.Success.ShouldBeTrue();
            result.Data.ShouldNotBeNull();
        }
        [TestMethod]
        public async Task ClientStore_Create_Delete()
        {
            var userId = Unique.S;
            var result = await _restHookClientManagementStore.FindHookUserClientsAsync(userId);
            result.ShouldNotBeNull();
            result.Data.ShouldBeNull();

            result = await _restHookClientManagementStore.CreateHookUserClientAsync(userId);
            result.ShouldNotBeNull();
            result.Data.ShouldNotBeNull();

            var result2 = await _restHookClientManagementStore.DeleteHookUserClientAsync(userId);
            result2.ShouldNotBeNull();
            result2.Success.ShouldBeTrue();

            result = await _restHookClientManagementStore.FindHookUserClientsAsync(userId);
            result.ShouldNotBeNull();
            result.Data.ShouldBeNull();
        }
        [TestMethod]
        public async Task ClientStore_Create_user_client_hookrecord_Delete()
        {
            var userId = Unique.S;
            var result = await _restHookClientManagementStore.FindHookUserClientsAsync(userId);
            result.ShouldNotBeNull();
            result.Data.ShouldBeNull();

            result = await _restHookClientManagementStore.CreateHookUserClientAsync(userId);
            result.ShouldNotBeNull();
            result.Data.ShouldNotBeNull();

            var hookUserClientsRecord = result.Data;
            var clientRecordResult = await _restHookClientManagementStore.CreateClientAsync(userId);
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

            result = await _restHookClientManagementStore.FindHookUserClientsAsync(userId);
            result.ShouldNotBeNull();
            result.Data.ShouldNotBeNull();
            var HookUserClientsRecord = result.Data;
            HookUserClientsRecord.Clients.Count.ShouldBe(1);
                
 
            var result2 = await _restHookClientManagementStore.DeleteHookUserClientAsync(userId);
            result2.ShouldNotBeNull();
            result2.Success.ShouldBeTrue();

            result = await _restHookClientManagementStore.FindHookUserClientsAsync(userId);
            result.ShouldNotBeNull();
            result.Data.ShouldBeNull();
        }
        [TestMethod]
        public async Task ClientStore_Delete_nonexistant_user()
        {
            var userId = Unique.S;
            var result = await _restHookClientManagementStore.FindHookUserClientsAsync(userId);
            result.ShouldNotBeNull();
            result.Data.ShouldBeNull();

            var result2 = await _restHookClientManagementStore.DeleteHookUserClientAsync(userId);
            result2.ShouldNotBeNull();
            result2.Success.ShouldBeTrue();

            result = await _restHookClientManagementStore.FindHookUserClientsAsync(userId);
            result.ShouldNotBeNull();
            result.Data.ShouldBeNull();
        }
        [TestMethod]
        public async Task ClientStore_Update_nonexistant_user()
        {
            var userId = Unique.S;
            // create recrod
            var result = await _restHookClientManagementStore.CreateHookUserClientAsync(userId);
            result.ShouldNotBeNull();
            result.Success.ShouldBeTrue();
            result.Data.ShouldNotBeNull();

            var hookUserClientsRecord = result.Data;

            // delete it
            var result2 = await _restHookClientManagementStore.DeleteHookUserClientAsync(userId);
            result2.ShouldNotBeNull();
            result2.Success.ShouldBeTrue();

            // still try to use it and update it
            var createClientResult = await _restHookClientManagementStore.CreateClientAsync(hookUserClientsRecord.UserId);
            createClientResult.ShouldNotBeNull();
            createClientResult.Success.ShouldBeFalse();

            result2 = await _restHookClientManagementStore.UpdateAsync(hookUserClientsRecord);
            result2.ShouldNotBeNull();
            result2.Success.ShouldBeFalse();
        }
        [TestMethod]
        public async Task ClientStore_Update_user()
        {
            var userId = Unique.S;
            // create recrod
            var record = await _restHookClientManagementStore.CreateHookUserClientAsync(userId);
            record.ShouldNotBeNull();


          

            // still try to use it and update it
            var clientRecordResult = await _restHookClientManagementStore.CreateClientAsync(record.Data.UserId);
            clientRecordResult.ShouldNotBeNull();
            clientRecordResult.Success.ShouldBeTrue();

            var clientRecord = clientRecordResult.Data;

            var result = await _restHookClientManagementStore.UpdateAsync(record.Data);
            result.ShouldNotBeNull();
            result.Success.ShouldBeTrue();

            var persitantRecord = await _restHookClientManagementStore.FindHookUserClientsAsync(userId);
            persitantRecord.ShouldNotBeNull();

            persitantRecord.Data.Clients.Count.ShouldBe(record.Data.Clients.Count);

            var culledClients = (persitantRecord.Data.Clients.Except(record.Data.Clients, new ClientRecordEqualityCompare())).ToList();
            culledClients.Count.ShouldBe(0);
        }

        [TestMethod]
        public async Task ClientStore_add_client_delete()
        {
            var userId = Unique.S;
            // create recrod
            var record = await _restHookClientManagementStore.CreateHookUserClientAsync(userId);
            record.ShouldNotBeNull();


            // still try to use it and update it
            var clientRecordResult = await _restHookClientManagementStore.CreateClientAsync(record.Data.UserId);
            clientRecordResult.ShouldNotBeNull();
            clientRecordResult.Success.ShouldBeTrue();
            var clientRecord = clientRecordResult.Data;


            var persitantRecord = await _restHookClientManagementStore.FindHookUserClientsAsync(userId);
            persitantRecord.ShouldNotBeNull();

            persitantRecord.Data.Clients.Count.ShouldBe(record.Data.Clients.Count);

            var culledClients = (persitantRecord.Data.Clients.Except(record.Data.Clients, new ClientRecordEqualityCompare())).ToList();
            culledClients.Count.ShouldBe(0);

            var client = persitantRecord.Data.Clients[0];

            var foundClient =
                await _restHookClientManagementStore.FindClientRecordAsync(userId, client.ClientId);
            foundClient.ShouldNotBeNull();
           
            foundClient.Data.ClientId.ShouldBe(client.ClientId);
            foundClient.Data.Description.ShouldBe(client.Description);


            var result = await _restHookClientManagementStore.DeleteHookUserClientRecordAsync(userId, persitantRecord.Data.Clients[0].ClientId);
            result.ShouldNotBeNull();
            result.Success.ShouldBeTrue();

            persitantRecord = await _restHookClientManagementStore.FindHookUserClientsAsync(userId);
            persitantRecord.ShouldNotBeNull();
            persitantRecord.Data.Clients.Count.ShouldBe(0);
        }
    }

    class ClientRecordEqualityCompare : IEqualityComparer<ClientRecord>
    {
        public bool Equals(ClientRecord x, ClientRecord y)
        {
            return x.ClientId == y.ClientId;
        }

        public int GetHashCode(ClientRecord obj)
        {
            return obj.ClientId.GetHashCode();
        }
    }
}