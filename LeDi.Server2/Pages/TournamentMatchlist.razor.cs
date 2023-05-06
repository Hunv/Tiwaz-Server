﻿using LeDi.Server2.DatabaseModel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Runtime.CompilerServices;

namespace LeDi.Server2.Pages
{
    public partial class TournamentMatchlist
    {
        private TblTournament Tournament { get; set; }

        /// <summary>
        /// Contains the roles a user has
        /// </summary>
        private TblUserRole? AuthenticatedUserRole { get; set; }


        [Parameter]
        public int? Id { get; set; }


        protected override async Task OnInitializedAsync()
        {
            // Get the roles of the currently logged in user
            var authState = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            if (authState != null && authState.User.Identity != null && authState.User.Identity.IsAuthenticated)
            {
                // Get the Roles from Identity management. Should only be one always.
                var username = authState.User.Identity.Name;
                if (username != null)
                {
                    var roles = await _UserManager.GetRolesAsync(await _UserManager.FindByNameAsync(username));

                    if (roles != null && roles.Count >= 1)
                    {
                        AuthenticatedUserRole = await DataHandler.GetUserRoleAsync(roles[0]);
                    }
                }
            }
            else
            {
                AuthenticatedUserRole = await DataHandler.GetUserRoleAsync("Guests");
            }

            if (!Id.HasValue)
                return;

            Tournament = await DataHandler.GetTournamentAsync(Id.Value);
        }

    }
}
