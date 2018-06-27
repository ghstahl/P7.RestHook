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
           Clients = new List<string>()
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
            var record = await _restHookClientManagementStore.FindHookUserClientAsync(Unique.S);
            record.ShouldBeNull();
        }
        [TestMethod]
        public async Task ClientStore_Not_Found_Create()
        {
            var userId = Unique.S;
            var record = await _restHookClientManagementStore.FindHookUserClientAsync(userId);
            record.ShouldBeNull();

            record = await _restHookClientManagementStore.CreateHookUserClientAsync(userId);
            record.ShouldNotBeNull();
        }
        [TestMethod]
        public async Task ClientStore_Create_Delete()
        {
            var userId = Unique.S;
            var record = await _restHookClientManagementStore.FindHookUserClientAsync(userId);
            record.ShouldBeNull();

            record = await _restHookClientManagementStore.CreateHookUserClientAsync(userId);
            record.ShouldNotBeNull();

            var result = await _restHookClientManagementStore.DeleteHookUserClientAsync(userId);
            result.ShouldNotBeNull();
            result.Success.ShouldBeTrue();

            record = await _restHookClientManagementStore.FindHookUserClientAsync(userId);
            record.ShouldBeNull();
        }
        [TestMethod]
        public async Task ClientStore_Delete_nonexistant_user()
        {
            var userId = Unique.S;
            var record = await _restHookClientManagementStore.FindHookUserClientAsync(userId);
            record.ShouldBeNull();

            var result = await _restHookClientManagementStore.DeleteHookUserClientAsync(userId);
            result.ShouldNotBeNull();
            result.Success.ShouldBeTrue();

            record = await _restHookClientManagementStore.FindHookUserClientAsync(userId);
            record.ShouldBeNull();
        }
        [TestMethod]
        public async Task ClientStore_Update_nonexistant_user()
        {
            var userId = Unique.S;
            // create recrod
            var record = await _restHookClientManagementStore.CreateHookUserClientAsync(userId);
            record.ShouldNotBeNull();

            // delete it
            var result = await _restHookClientManagementStore.DeleteHookUserClientAsync(userId);
            result.ShouldNotBeNull();
            result.Success.ShouldBeTrue();

            // still try to use it and update it
            result = await _restHookClientManagementStore.AddClientAsync(record, Unique.S);
            result.ShouldNotBeNull();
            result.Success.ShouldBeTrue();

            result = await _restHookClientManagementStore.UpdateAsync(record);
            result.ShouldNotBeNull();
            result.Success.ShouldBeFalse();
        }
        [TestMethod]
        public async Task ClientStore_Update_user()
        {
            var userId = Unique.S;
            // create recrod
            var record = await _restHookClientManagementStore.CreateHookUserClientAsync(userId);
            record.ShouldNotBeNull();

            var clientId = Unique.S;

            // still try to use it and update it
            var result = await _restHookClientManagementStore.AddClientAsync(record, clientId);
            result.ShouldNotBeNull();
            result.Success.ShouldBeTrue();

            result = await _restHookClientManagementStore.UpdateAsync(record);
            result.ShouldNotBeNull();
            result.Success.ShouldBeTrue();

            var persitantRecord = await _restHookClientManagementStore.FindHookUserClientAsync(userId);
            persitantRecord.ShouldNotBeNull();

            persitantRecord.Clients.Count.ShouldBe(record.Clients.Count);

            var culledClients =
                (persitantRecord.Clients.Where(t2 => !record.Clients.Any(t1 => t2.Contains(t1)))).ToList();
            culledClients.Count.ShouldBe(0);

        }
    }
}