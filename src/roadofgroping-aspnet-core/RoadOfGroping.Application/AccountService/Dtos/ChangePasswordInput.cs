namespace RoadOfGroping.Application.AccountService.Dtos
{
    public class ChangePasswordInput
    {
        public string OldPassword { get; set; }

        public string NewPassword { get; set; }

        public string ConfirmPassword { get; set; }
    }
}