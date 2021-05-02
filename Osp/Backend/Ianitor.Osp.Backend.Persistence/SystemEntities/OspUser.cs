using System.Collections.Generic;
using AspNetCore.Identity.Mongo.Model;
using MongoDB.Bson;

namespace Ianitor.Osp.Backend.Persistence.SystemEntities
{
    /// <summary>
    /// Represents an user identity.
    /// </summary>
    /// <remarks>
    /// Add profile data for application users by adding properties to the IdentityUser class
    /// </remarks>
    [CollectionName("Users")]
    public class OspUser : MongoUser
    {
      /// <summary>
      /// The first name of the user.
      /// </summary>
      public string FirstName { get; set; }

      /// <summary>
      /// The last name of the user.
      /// </summary>
      public string LastName { get; set; }

      /// <summary>
      /// Force the user to change the password after the next login.
      /// </summary>
      public bool ResetPasswordOnLogin { get; set; }

      /// <summary>
      /// Further information on the user.
      /// </summary>
      public string Description { get; set; }
    }
}
