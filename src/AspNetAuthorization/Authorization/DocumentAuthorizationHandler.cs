﻿using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;

using AspNetAuthorization.Models;

namespace AspNetAuthorization.Authorization
{
    public class AdminAuthorizationHander : AuthorizationHandler<OperationAuthorizationRequirement, Document>
    {
        
        protected override void Handle(AuthorizationContext context, OperationAuthorizationRequirement requirement, Document resource)
        {
            var isSuperUser = context.User.FindFirst(c => c.Type == "Superuser" && c.Issuer == Issuers.Idunno && c.Value == "True");
            if (isSuperUser != null)
            {
                context.Succeed(requirement);
                return;
            }
        }
    }

    public class IHateYouAuthorizationHander : AuthorizationHandler<OperationAuthorizationRequirement, Document>
    {
        protected override void Handle(AuthorizationContext context, OperationAuthorizationRequirement requirement, Document resource)
        {
            if (context.User.Identity.Name == "davidfowler")
            {
                context.Fail();
                return;
            }
        }
    }

    public class DocumentAuthorizationHandler : AuthorizationHandler<OperationAuthorizationRequirement, Document>
    {
        protected override void Handle(AuthorizationContext context, OperationAuthorizationRequirement requirement, Document resource)
        {
            var documentPermissionClaim = context.User.FindFirst(c => c.Type == "Documents" && c.Issuer == "urn:idunno.org");

            if (MapClaimsToOperations(documentPermissionClaim.Value).Contains(requirement))
            {
                context.Succeed(requirement);
            }
        }

        private IEnumerable<OperationAuthorizationRequirement> MapClaimsToOperations(string claimValue)
        {
            var mappedPermissions = new List<OperationAuthorizationRequirement>();

            if (claimValue.Contains('C'))
            {
                mappedPermissions.Add(Operations.Create);
            }
            if (claimValue.Contains('R'))
            {
                mappedPermissions.Add(Operations.Read);
            }
            if (claimValue.Contains('U'))
            {
                mappedPermissions.Add(Operations.Update);
            }
            if (claimValue.Contains('D'))
            {
                mappedPermissions.Add(Operations.Delete);
            }

            return mappedPermissions;
        }
    }
}