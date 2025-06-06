namespace TuneSpace.Core.Interfaces.IServices;

/// <summary>
/// Service interface for loading and processing email templates.
/// Provides functionality to load template files and merge them with data models
/// to generate personalized email content.
/// </summary>
public interface IEmailTemplateService
{
    /// <summary>
    /// Loads a raw email template by name.
    /// </summary>
    /// <param name="templateName">The name of the template file to load (without extension).</param>
    /// <returns>The raw template content as a string.</returns>
    /// <exception cref="FileNotFoundException">Thrown when the specified template file cannot be found.</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown when access to the template file is denied.</exception>
    string LoadTemplate(string templateName);

    /// <summary>
    /// Processes an email template by merging it with a data model.
    /// This method loads the specified template and replaces placeholders with values from the provided model.
    /// </summary>
    /// <param name="templateName">The name of the template file to process (without extension).</param>
    /// <param name="model">The data model containing values to merge into the template. Properties of this object will be used to replace placeholders in the template.</param>
    /// <returns>The processed template with placeholders replaced by model values.</returns>
    /// <exception cref="FileNotFoundException">Thrown when the specified template file cannot be found.</exception>
    /// <exception cref="UnauthorizedAccessException">Thrown when access to the template file is denied.</exception>
    /// <exception cref="ArgumentNullException">Thrown when templateName is null or empty, or when model is null.</exception>
    string ProcessTemplate(string templateName, object model);
}
