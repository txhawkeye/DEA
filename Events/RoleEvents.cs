using DEA.Services;
using Discord;
using Discord.WebSocket;
using System.Threading.Tasks;

namespace DEA.Events
{
    class RoleEvents
    {
        private DiscordSocketClient _client;

        public RoleEvents(DiscordSocketClient client)
        {
            _client = client;
            _client.RoleCreated += HandleRoleCreated;
            _client.RoleUpdated += HandleRoleUpdated;
            _client.RoleDeleted += HandleRoleDeleted;
        }

        private async Task HandleRoleCreated(SocketRole role)
        {
            await Logger.DetailedLog(role.Guild, "Action", "Role Creation", "Role", role.Name, role.Id, new Color(12, 255, 129));
        }

        private async Task HandleRoleUpdated(SocketRole roleBefore, SocketRole roleAfter)
        {
            await Logger.DetailedLog(roleAfter.Guild, "Action", "Role Modification", "Role", roleAfter.Name, roleAfter.Id, new Color(12, 255, 129));
        }

        private async Task HandleRoleDeleted(SocketRole role)
        {
            await Logger.DetailedLog(role.Guild, "Action", "Role Deletion", "Role", role.Name, role.Id, new Color(255, 0, 0));
        }
    }
}
