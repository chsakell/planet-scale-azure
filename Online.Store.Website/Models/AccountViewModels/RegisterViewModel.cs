using System.ComponentModel.DataAnnotations;

namespace Online.Store.Website.Models.AccountViewModels
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email", Prompt ="Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password", Prompt = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password", Prompt = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Name", Prompt = "Name")]
        public string FullName { get; set; }

        [Required]
        [Display(Name = "City", Prompt = "City")]
        public string City { get; set; }

        [Display(Name = "State", Prompt = "State")]
        public string State { get; set; }

        [Display(Name = "Twitter Handle", Prompt = "Twitter Handle")]
        public string TwitterHandle { get; set; }

        //public string Avatar { get; set; }
        public string ImageUrl { get; set; }

        public bool Notifications { get; set; }
        public bool ProductNotification { get; set; }
        public bool PostNotification { get; set; }
        public bool TweetNotification { get; set; }
        public bool TermsService { get; set; }
    }
}
