using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
// ReSharper disable UnusedMember.Global

namespace Ianitor.Osp.Common.Shared.DataTransferObjects
{
    public class UserDto
    {
        /// <summary>
        /// Gets or sets the first name
        /// </summary>
        public string FirstName { get; set; }
        
        /// <summary>
        /// Gets or sets the last name
        /// </summary>
        public string LastName { get; set; }
        
        /// <summary>
        /// Gets or sets the user id
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        /// Gets or sets the E-Mail address of the user
        /// </summary>
        [Required]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the display name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets roles of the user
        /// </summary>
        [Required]
        public IEnumerable<RoleDto> Roles { get; set; }
    }
}
