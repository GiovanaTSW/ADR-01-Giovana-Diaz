namespace Dressly.Application.Services;

public class ModelSheetRequest
{
    public string SourceImagePath { get; set; } = string.Empty;

    public string PromptText =>
        "Act as an expert character designer and 2D model artist. Analyze the provided image and create a comprehensive 2D Model Sheet of the same male subject, maintaining his exact style, pose, and proportions. " +
        "Input: Referencing the attached image, the model sheet must include: " +
        "Main Pose: A full-body 2D illustration of the subject in the exact pose from the reference image, including all his clothing (light-wash jeans, blue pinstripe shirt, cream blazer, and black loafers) and his glasses. " +
        "Head Rotations: Detailed 2D illustrations of the subject's head from at least three angles: Front view, Side profile view, Three-quarter view. " +
        "Expression and Accessory Sheet: Focus on the face and accessories: " +
        "Face: Close-up views of his face with neutral, smiling, and serious expressions. " +
        "Glasses: A detailed view of his specific glasses design, showing front, side, and folded angles. " +
        "Output Style: The model sheet should be a clean, professional reference, similar to a film or video game character sheet. " +
        "Format: Render the images in a flat or cell-shaded style, suitable for easy asset swapping and later adding clothes. " +
        "Arrange all elements clearly on a clean white background with a simple title like 'MALE CHARACTER MODEL SHEET - DRESSLY REFERENCE'. " +
        "Ensure the subject's likeness, physique, and height remain consistent across all views.";
}