using System;
using System.Threading.Tasks;
using StudioX.Dependency;
using StudioX.RealTime;
using StudioX.Runtime.Session;
using Castle.Core.Logging;
using Microsoft.AspNet.SignalR;

namespace StudioX.Web.SignalR.Hubs
{
    /// <summary>
    /// Common Hub of StudioX.
    /// </summary>
    public class StudioXCommonHub : Hub, ITransientDependency
    {
        /// <summary>
        /// Reference to the logger.
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// Reference to the session.
        /// </summary>
        public IStudioXSession StudioXSession { get; set; }

        private readonly IOnlineClientManager _onlineClientManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="StudioXCommonHub"/> class.
        /// </summary>
        public StudioXCommonHub(IOnlineClientManager onlineClientManager)
        {
            _onlineClientManager = onlineClientManager;

            Logger = NullLogger.Instance;
            StudioXSession = NullStudioXSession.Instance;
        }

        public void Register()
        {
            Logger.Debug("A client is registered: " + Context.ConnectionId);
        }

        public override async Task OnConnected()
        {
            await base.OnConnected();

            var client = CreateClientForCurrentConnection();

            Logger.Debug("A client is connected: " + client);
            
            _onlineClientManager.Add(client);
        }

        public override async Task OnReconnected()
        {
            await base.OnReconnected();

            var client = _onlineClientManager.GetByConnectionIdOrNull(Context.ConnectionId);
            if (client == null)
            {
                client = CreateClientForCurrentConnection();
                _onlineClientManager.Add(client);
                Logger.Debug("A client is connected (on reconnected event): " + client);
            }
            else
            {
                Logger.Debug("A client is reconnected: " + client);
            }
        }

        public override async Task OnDisconnected(bool stopCalled)
        {
            await base.OnDisconnected(stopCalled);

            Logger.Debug("A client is disconnected: " + Context.ConnectionId);

            try
            {
                _onlineClientManager.Remove(Context.ConnectionId);
            }
            catch (Exception ex)
            {
                Logger.Warn(ex.ToString(), ex);
            }
        }

        private IOnlineClient CreateClientForCurrentConnection()
        {
            return new OnlineClient(
                Context.ConnectionId,
                GetIpAddressOfClient(),
                StudioXSession.TenantId,
                StudioXSession.UserId
            );
        }

        private string GetIpAddressOfClient()
        {
            try
            {
                return Context.Request.Environment["server.RemoteIpAddress"].ToString();
            }
            catch (Exception ex)
            {
                Logger.Error("Can not find IP address of the client! connectionId: " + Context.ConnectionId);
                Logger.Error(ex.Message, ex);
                return "";
            }
        }
    }
}
