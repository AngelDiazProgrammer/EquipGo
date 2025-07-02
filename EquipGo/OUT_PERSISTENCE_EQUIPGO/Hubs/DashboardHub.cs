using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OUT_PERSISTENCE_EQUIPGO.Hubs
{
    public class DashboardHub : Hub
    {
        // Solo señalizamos al frontend que hay nuevas transacciones
        public async Task NotificarNuevaTransaccionVisitante()
        {
            await Clients.All.SendAsync("NuevaTransaccionVisitante");
        }
    }
}
