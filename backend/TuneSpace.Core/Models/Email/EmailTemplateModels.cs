namespace TuneSpace.Core.Models.Email;

public record EmailConfirmationModel(string Username, string ConfirmationUrl, string Year);

public record WelcomeEmailModel(string Username, string Year);

public record PasswordResetModel(string Username, string ResetUrl, string Year);

public record EmailChangeConfirmationModel(string Username, string NewEmail, string ConfirmationUrl, string Year);
