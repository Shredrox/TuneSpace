namespace TuneSpace.Core.Interfaces.IServices;

/// <summary>
/// Service for managing OAuth state tokens to prevent CSRF attacks.
/// </summary>
public interface IOAuthStateService
{
    /// <summary>
    /// Generates and stores a new OAuth state token.
    /// </summary>
    /// <returns>The generated state token.</returns>
    string GenerateAndStoreState();

    /// <summary>
    /// Validates if the provided state token is valid and removes it from storage.
    /// </summary>
    /// <param name="state">The state token to validate.</param>
    /// <returns>True if the state is valid, false otherwise.</returns>
    bool ValidateAndConsumeState(string state);
}
