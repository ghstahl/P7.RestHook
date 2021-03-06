﻿using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using P7.RestHook.Models;
using P7.RestHook.Store;
using Shouldly;

namespace UnitTest.RestHookStore.Core.Stores
{
    public abstract class UnitTestRestHookStore
    {
        private IRestHookStore _restHookStore;
        private IRestHookStoreTest _restHookStoreTest;

        private static HookRecord UniqueHookRecord => new HookRecord
        {
            Id = Unique.G,
            ClientId = Unique.G,
            CallbackUrl = Unique.Url,
            EventName = Unique.S,
            ValidatedCallbackUrl = false
        };

        public UnitTestRestHookStore(IRestHookStoreTest restHookStoreTest,IRestHookStore restHookStore)
        {
            _restHookStoreTest = restHookStoreTest;
            _restHookStore = restHookStore;
        }
        [TestInitialize]
        public async Task Initialize()
        {
            await _restHookStoreTest.DropAsync();
        }
        [TestMethod]
        public async Task DI_Valid()
        {
            _restHookStore.ShouldNotBeNull();
        }

        [TestMethod]
        public async Task Upsert_Add_Success()
        {
            var record = UniqueHookRecord;
            var result = await _restHookStore.UpsertAsync(record);
            result.ShouldNotBeNull();
            result.Success.ShouldBeTrue();
        }

        [TestMethod]
        public async Task Upsert_Update_Success()
        {
            

            var record = UniqueHookRecord;
            var result = await _restHookStore.UpsertAsync(record);
            result.ShouldNotBeNull();
            result.Success.ShouldBeTrue();

            var res2 = await _restHookStore.FindByIdAsync(record.Id);
            res2.ShouldNotBeNull();
            res2.Success.ShouldBeTrue();
            res2.Data.ClientId.ShouldBe(record.ClientId);

            var record2 = UniqueHookRecord;
            record2.Id = record.Id;
            result = await _restHookStore.UpsertAsync(record2);
            result.ShouldNotBeNull();
            result.Success.ShouldBeTrue();

            res2 = await _restHookStore.FindByIdAsync(record.Id);
            res2.ShouldNotBeNull();
            res2.Success.ShouldBeTrue();
            res2.Data.ClientId.ShouldBe(record2.ClientId);
        }

        [TestMethod]
        public async Task Upsert_Page_Success()
        {
            
            var record = UniqueHookRecord;
            var result = await _restHookStore.UpsertAsync(record);
            result.ShouldNotBeNull();
            result.Success.ShouldBeTrue();

            var page = await _restHookStore.PageAsync(100, null);
            page.ShouldNotBeNull();
            page.Data.CurrentPagingState.ShouldBeNull();

            page.Data.Count.ShouldBe(1);
        }
       
        [TestMethod]
        public async Task Upsert_Page_Delete_Success()
        {
            
            var record = UniqueHookRecord;
            var result = await _restHookStore.UpsertAsync(record);
            result.ShouldNotBeNull();
            result.Success.ShouldBeTrue();

            var page = await _restHookStore.PageAsync(100, null);
            page.ShouldNotBeNull();
            page.Data.CurrentPagingState.ShouldBeNull();
            page.Data.Count.ShouldBe(1);

            result = await _restHookStore.DeleteAsync(record);
            result.ShouldNotBeNull();
            result.Success.ShouldBeTrue();

            page = await _restHookStore.PageAsync(100, null);
            page.ShouldNotBeNull();
            page.Data.CurrentPagingState.ShouldBeNull();
            page.Data.Count.ShouldBe(0);
        }
        [TestMethod]
        public async Task Upsert_ManySameEvent_SameClient_Page_Delete_Success()
        {
            
            var record = UniqueHookRecord;
            var result = await _restHookStore.UpsertAsync(record);
            result.ShouldNotBeNull();
            result.Success.ShouldBeTrue();

            record.Id = Unique.G;
            result = await _restHookStore.UpsertAsync(record);
            result.ShouldNotBeNull();
            result.Success.ShouldBeTrue();

            var page = await _restHookStore.PageAsync(100, null);
            page.ShouldNotBeNull();
            page.Data.CurrentPagingState.ShouldBeNull();
            page.Data.Count.ShouldBe(2);

            record.Id = null;
            result = await _restHookStore.DeleteAsync(record);
            result.ShouldNotBeNull();
            result.Success.ShouldBeTrue();

            page = await _restHookStore.PageAsync(100, null);
            page.ShouldNotBeNull();
            page.Data.CurrentPagingState.ShouldBeNull();
            page.Data.Count.ShouldBe(0);
        }
        [TestMethod]
        public async Task Upsert_ManySameEvent_Page_Delete_Fail()
        {
            
            var eventName = Unique.S;
            var count = 0;
            for (int i = 0; i < 10; ++i)
            {
                var rc = UniqueHookRecord;
                rc.EventName = eventName;
                var resultF = await _restHookStore.UpsertAsync(rc);
                resultF.ShouldNotBeNull();
                resultF.Success.ShouldBeTrue();
                ++count;
            }

            var page = await _restHookStore.PageAsync(100, null);
            page.ShouldNotBeNull();
            page.Data.CurrentPagingState.ShouldBeNull();
            page.Data.Count.ShouldBe(count);


            var result = await _restHookStore.DeleteAsync(new HookRecord()
            {
                EventName = eventName
            });
            result.ShouldNotBeNull();
            result.Success.ShouldBeFalse();

            page = await _restHookStore.PageAsync(100, null);
            page.ShouldNotBeNull();
            page.Data.CurrentPagingState.ShouldBeNull();
            page.Data.Count.ShouldBe(count);
        }
        [TestMethod]
        public async Task Upsert_Many_SameClient_Success()
        {
            
            var clientId = Unique.S;
            var count = 0;
            for (int i = 0; i < 10; ++i)
            {
                var rc = UniqueHookRecord;
                rc.ClientId = clientId;
                var resultF = await _restHookStore.UpsertAsync(rc);
                resultF.ShouldNotBeNull();
                resultF.Success.ShouldBeTrue();
                ++count;
            }
            // Throw in some random ones as well.
            for (int i = 0; i < 10; ++i)
            {
                var rc = UniqueHookRecord;
                var resultF = await _restHookStore.UpsertAsync(rc);
                resultF.ShouldNotBeNull();
                resultF.Success.ShouldBeTrue();
                ++count;
            }
            var page = await _restHookStore.PageAsync(100, null);
            page.ShouldNotBeNull();
            page.Data.CurrentPagingState.ShouldBeNull();
            page.Data.Count.ShouldBe(count);

            var record = new HookRecordQuery()
            {
                ClientId = clientId
            };


            page = await _restHookStore.PageAsync(record, 100, null);
            page.ShouldNotBeNull();
            page.Data.CurrentPagingState.ShouldBeNull();
            page.Data.Count.ShouldBe(10);


        }
        [TestMethod]
        public async Task Upsert_Many_SameClient_delete_Success()
        {
            
            var clientId = Unique.S;
            var count = 0;
            for (int i = 0; i < 10; ++i)
            {
                var rc = UniqueHookRecord;
                rc.ClientId = clientId;
                var resultF = await _restHookStore.UpsertAsync(rc);
                resultF.ShouldNotBeNull();
                resultF.Success.ShouldBeTrue();
                ++count;
            }
            // Throw in some random ones as well.
            for (int i = 0; i < 10; ++i)
            {
                var rc = UniqueHookRecord;
                var resultF = await _restHookStore.UpsertAsync(rc);
                resultF.ShouldNotBeNull();
                resultF.Success.ShouldBeTrue();
                ++count;
            }
            var page = await _restHookStore.PageAsync(100, null);
            page.ShouldNotBeNull();
            page.Data.CurrentPagingState.ShouldBeNull();
            page.Data.Count.ShouldBe(count);

            var record = new HookRecordQuery()
            {
                ClientId = clientId
            };

            var recordDelete = new HookRecord {ClientId = clientId};
            var result = await _restHookStore.DeleteAsync(recordDelete);
            result.ShouldNotBeNull();
            result.Success.ShouldBeTrue();

            page = await _restHookStore.PageAsync(record, 100, null);
            page.ShouldNotBeNull();
            page.Data.CurrentPagingState.ShouldBeNull();
            page.Data.Count.ShouldBe(0);


        }

        [TestMethod]
        public async Task Upsert_Many_Client_SameEvent_Success()
        {
            
            var eventName = Unique.S;
            var count = 0;
            for (int i = 0; i < 10; ++i)
            {
                var rc = UniqueHookRecord;
                rc.EventName = eventName;
                var resultF = await _restHookStore.UpsertAsync(rc);
                resultF.ShouldNotBeNull();
                resultF.Success.ShouldBeTrue();
                ++count;
            }
            // Throw in some random ones as well.
            for (int i = 0; i < 10; ++i)
            {
                var rc = UniqueHookRecord;
                var resultF = await _restHookStore.UpsertAsync(rc);
                resultF.ShouldNotBeNull();
                resultF.Success.ShouldBeTrue();
                ++count;
            }
            var page = await _restHookStore.PageAsync(100, null);
            page.ShouldNotBeNull();
            page.Data.CurrentPagingState.ShouldBeNull();
            page.Data.Count.ShouldBe(count);

            var record = new HookRecordQuery()
            {
                EventName = eventName
            };


            page = await _restHookStore.PageAsync(record, 100, null);
            page.ShouldNotBeNull();
            page.Data.CurrentPagingState.ShouldBeNull();
            page.Data.Count.ShouldBe(10);


        }

        [TestMethod]
        public async Task Upsert_Many_SameClient_onepage_Success()
        {
            
            var clientId = Unique.S;
            var eventName = Unique.S;
            var rc = UniqueHookRecord;
            rc.ClientId = clientId;
            rc.EventName = eventName;
            var resultF = await _restHookStore.UpsertAsync(rc);
            resultF.ShouldNotBeNull();
            resultF.Success.ShouldBeTrue();

            var count = 1;

            for (int i = 0; i < 10; ++i)
            {
                rc = UniqueHookRecord;
                rc.ClientId = clientId;
                resultF = await _restHookStore.UpsertAsync(rc);
                resultF.ShouldNotBeNull();
                resultF.Success.ShouldBeTrue();
                ++count;
            }
            // Throw in some random ones as well.
            for (int i = 0; i < 10; ++i)
            {
                rc = UniqueHookRecord;
                resultF = await _restHookStore.UpsertAsync(rc);
                resultF.ShouldNotBeNull();
                resultF.Success.ShouldBeTrue();
                ++count;
            }
            var page = await _restHookStore.PageAsync(100, null);
            page.ShouldNotBeNull();
            page.Data.CurrentPagingState.ShouldBeNull();
            page.Data.Count.ShouldBe(count);

            var record = new HookRecordQuery()
            {
                ClientId = clientId,
                EventName = eventName
            };


            page = await _restHookStore.PageAsync(record, 100, null);
            page.ShouldNotBeNull();
            page.Data.CurrentPagingState.ShouldBeNull();
            page.Data.Count.ShouldBe(1);


        }
    }
}
