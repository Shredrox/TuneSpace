using System.Text;
using TuneSpace.Core.Interfaces.IServices;
using TuneSpace.Core.Models;

namespace TuneSpace.Application.Services.AI;

internal class EnhancedAIPromptService : IEnhancedAIPromptService
{
    string IEnhancedAIPromptService.BuildAdvancedContextAwarePrompt(
        List<string> topArtists,
        List<string> genres,
        string location,
        List<BandModel> vectorResults,
        string ragContext,
        Dictionary<string, object> userBehaviorContext)
    {
        var prompt = new StringBuilder();

        prompt.AppendLine("You are an expert music discovery AI with deep understanding of underground music ecosystems, genre evolution, and personalized taste modeling.");
        prompt.AppendLine("CORE MISSION: Discover hidden gems and emerging artists that perfectly match user preferences while introducing meaningful musical exploration.");
        prompt.AppendLine();

        prompt.AppendLine("USER PROFILE:");
        prompt.AppendLine($"📍 Location: {location}");
        prompt.AppendLine($"🎵 Top Artists: {string.Join(", ", topArtists)}");
        prompt.AppendLine($"🎭 Genre Preferences: {string.Join(", ", genres)}");

        if (userBehaviorContext != null && userBehaviorContext.Count > 0)
        {
            prompt.AppendLine("🧠 Behavioral Patterns:");
            foreach (var behavior in userBehaviorContext)
            {
                prompt.AppendLine($"  - {behavior.Key}: {behavior.Value}");
            }
        }
        prompt.AppendLine();

        prompt.AppendLine("🔍 VECTOR SEARCH DISCOVERIES (AI-identified similar artists):");
        foreach (var band in vectorResults.Take(8))
        {
            prompt.AppendLine($"• {band.Name}");
            prompt.AppendLine($"  └ Genres: {string.Join(", ", band.Genres)}");
            prompt.AppendLine($"  └ Origin: {band.Location}");
            prompt.AppendLine($"  └ Popularity: {GetPopularityDescription(band.Popularity)}");
            if (!string.IsNullOrEmpty(band.Description) && band.Description.Length > 20)
                prompt.AppendLine($"  └ Profile: {(band.Description.Length > 120 ? band.Description[..120] + "..." : band.Description)}");
            prompt.AppendLine();
        }

        prompt.AppendLine("📚 CONTEXTUAL MUSIC KNOWLEDGE:");
        prompt.AppendLine(ragContext);
        prompt.AppendLine();

        prompt.AppendLine("🎯 DISCOVERY OBJECTIVES:");
        prompt.AppendLine("1. **Pattern Analysis**: Identify musical DNA patterns from vector results");
        prompt.AppendLine("2. **Genre Bridge Building**: Find artists that connect user's current tastes with unexplored territories");
        prompt.AppendLine("3. **Local Scene Exploration**: Prioritize underground artists from user's geographic region");
        prompt.AppendLine("4. **Temporal Relevance**: Consider both classic influences and contemporary evolution");
        prompt.AppendLine("5. **Authenticity Filter**: Focus on artists with genuine artistic vision over commercial appeal");
        prompt.AppendLine("6. **Discovery Gradient**: Balance familiar-adjacent recommendations with bold explorations");
        prompt.AppendLine();

        prompt.AppendLine("🚨 OUTPUT FORMAT (STRICTLY ENFORCED):");
        prompt.AppendLine("You must return 8 to 12 recommendations.");
        prompt.AppendLine("Each suggestion should represent a strategic discovery opportunity");
        prompt.AppendLine("Consider cross-genre pollination and scene connections");
        prompt.AppendLine("Prioritize artists with less than 100k monthly listeners when possible");
        prompt.AppendLine("Each recommendation must appear on a SINGLE LINE.");
        prompt.AppendLine("Each line MUST follow this **EXACT FORMAT** (or it will be discarded):");
        prompt.AppendLine();
        prompt.AppendLine("Artist Name [Confidence: X] - Brief explanation of musical relevance.");
        prompt.AppendLine();
        prompt.AppendLine("✅ Example:");
        prompt.AppendLine("Obsidian [Confidence: 8] - Blends djent and industrial with atmospheric synths; rhythmically akin to Sleep Token.");
        prompt.AppendLine();
        prompt.AppendLine("❌ DO NOT use numbering, bullets, multiple lines per artist, or any other format.");
        prompt.AppendLine("❌ DO NOT include markdown formatting or extra spacing.");
        prompt.AppendLine();
        prompt.AppendLine("If you cannot format your response in this exact way, do not generate a response.");

        return prompt.ToString();
    }

    string IEnhancedAIPromptService.BuildAdaptiveRankingPrompt(
        List<BandModel> candidates,
        List<string> topArtists,
        List<string> genres,
        string location,
        string userJourneyStage,
        Dictionary<string, double>? genreWeights)
    {
        var prompt = new StringBuilder();

        prompt.AppendLine("You are an advanced music recommendation engine with deep learning capabilities.");
        prompt.AppendLine($"CURRENT USER JOURNEY STAGE: {userJourneyStage.ToUpper()}");
        prompt.AppendLine();

        switch (userJourneyStage.ToLower())
        {
            case "discovery":
                prompt.AppendLine("🌱 DISCOVERY MODE: User is in active exploration phase - prioritize diverse, boundary-pushing recommendations");
                break;
            case "deepening":
                prompt.AppendLine("🔬 DEEPENING MODE: User wants to explore variations within established preferences");
                break;
            case "comfort":
                prompt.AppendLine("🏠 COMFORT MODE: User seeks familiar but high-quality recommendations");
                break;
            default:
                prompt.AppendLine("⚡ EXPLORATION MODE: Balanced approach between discovery and familiarity");
                break;
        }
        prompt.AppendLine();

        prompt.AppendLine("USER CONTEXT:");
        prompt.AppendLine($"🎵 Core Artists: {string.Join(", ", topArtists)}");
        prompt.AppendLine($"🎭 Genre Palette: {string.Join(", ", genres)}");
        prompt.AppendLine($"📍 Location: {location}");

        if (genreWeights != null && genreWeights.Count > 0)
        {
            prompt.AppendLine("📊 Genre Affinity Weights:");
            foreach (var weight in genreWeights.OrderByDescending(w => w.Value))
            {
                prompt.AppendLine($"  • {weight.Key}: {weight.Value:F2}");
            }
        }
        prompt.AppendLine();

        prompt.AppendLine("CANDIDATE ARTISTS FOR RANKING:");
        for (int i = 0; i < candidates.Count; i++)
        {
            var band = candidates[i];
            prompt.AppendLine($"{i + 1}. {band.Name}");
            prompt.AppendLine($"   └ Genres: {string.Join(", ", band.Genres)}");
            prompt.AppendLine($"   └ Location: {band.Location}");
            prompt.AppendLine($"   └ Discovery Score: {band.RelevanceScore:F2}");
            prompt.AppendLine($"   └ Source: {band.DataSource}");
        }
        prompt.AppendLine();

        prompt.AppendLine("🎯 RANKING CRITERIA (weighted by journey stage):");
        prompt.AppendLine("1. **Musical Compatibility**: How well does the artist align with user's taste profile?");
        prompt.AppendLine("2. **Discovery Value**: Does this artist offer meaningful musical exploration?");
        prompt.AppendLine("3. **Geographic Relevance**: Preference for local/regional artists when quality matches");
        prompt.AppendLine("4. **Scene Connectivity**: How well does this artist connect user to broader musical communities?");
        prompt.AppendLine("5. **Artistic Authenticity**: Is this artist driving genuine creative innovation?");
        prompt.AppendLine("6. **Accessibility Bridge**: Does this artist provide a comfortable entry point to new sounds?");
        prompt.AppendLine();

        prompt.AppendLine("OUTPUT FORMAT: Provide ranked list with scores:");
        prompt.AppendLine("1. Artist Name [Score: X.X] - Brief justification");
        prompt.AppendLine("2. Artist Name [Score: X.X] - Brief justification");
        prompt.AppendLine("...");

        return prompt.ToString();
    }

    string IEnhancedAIPromptService.BuildEnhancedExplanationPrompt(
        BandModel band,
        List<string> userGenres,
        List<string> userTopArtists,
        string recommendationContext)
    {
        var prompt = new StringBuilder();

        prompt.AppendLine("You are a knowledgeable music curator explaining why a specific artist recommendation is perfect for this user.");
        prompt.AppendLine("Create a compelling, personalized explanation that helps the user understand the musical connection.");
        prompt.AppendLine();

        prompt.AppendLine("USER'S MUSICAL IDENTITY:");
        prompt.AppendLine($"🎵 Favorite Artists: {string.Join(", ", userTopArtists)}");
        prompt.AppendLine($"🎭 Genre Preferences: {string.Join(", ", userGenres)}");
        prompt.AppendLine();

        prompt.AppendLine("RECOMMENDED ARTIST:");
        prompt.AppendLine($"🎤 Name: {band.Name}");
        prompt.AppendLine($"🎭 Genres: {string.Join(", ", band.Genres)}");
        prompt.AppendLine($"📍 Location: {band.Location}");
        prompt.AppendLine($"⭐ Popularity Level: {GetPopularityDescription(band.Popularity)}");
        prompt.AppendLine($"🔍 Discovery Source: {band.DataSource}");

        if (!string.IsNullOrEmpty(band.Description))
        {
            prompt.AppendLine($"📝 Artist Profile: {band.Description}");
        }

        if (!string.IsNullOrEmpty(recommendationContext))
        {
            prompt.AppendLine($"🎯 Recommendation Context: {recommendationContext}");
        }
        prompt.AppendLine();

        prompt.AppendLine("EXPLANATION OBJECTIVES:");
        prompt.AppendLine("1. **Musical Bridges**: Explain specific connections to user's existing preferences");
        prompt.AppendLine("2. **Discovery Value**: Highlight what new musical territories this opens");
        prompt.AppendLine("3. **Emotional Resonance**: Connect to the emotional qualities the user enjoys");
        prompt.AppendLine("4. **Cultural Context**: If relevant, explain the artist's place in music scenes/movements");
        prompt.AppendLine("5. **Personal Growth**: How this recommendation supports the user's musical journey");
        prompt.AppendLine();

        prompt.AppendLine("FORMAT: Write 2-3 engaging sentences that feel personal and insightful.");
        prompt.AppendLine("TONE: Enthusiastic but knowledgeable, like a trusted friend sharing a musical discovery.");

        return prompt.ToString();
    }

    string IEnhancedAIPromptService.BuildEnhancedPrompt(
        List<string> genres,
        string location,
        List<BandModel> vectorResults,
        string ragContext)
    {
        var promptBuilder = new StringBuilder();

        promptBuilder.AppendLine("You are an expert music curator specializing in underground and emerging artists.");
        promptBuilder.AppendLine("TASK: Analyze the provided context and recommend additional underground bands.");
        promptBuilder.AppendLine();

        promptBuilder.AppendLine($"USER PROFILE:");
        promptBuilder.AppendLine($"- Location: {location}");
        promptBuilder.AppendLine($"- Preferred Genres: {string.Join(", ", genres)}");
        promptBuilder.AppendLine();

        promptBuilder.AppendLine("VECTOR SEARCH RESULTS (AI-found similar artists):");
        foreach (var band in vectorResults.Take(10))
        {
            promptBuilder.AppendLine($"- {band.Name}");
            promptBuilder.AppendLine($"  Genres: {string.Join(", ", band.Genres)}");
            promptBuilder.AppendLine($"  Location: {band.Location}");
            if (!string.IsNullOrEmpty(band.Description))
                promptBuilder.AppendLine($"  Description: {(band.Description.Length > 150 ? band.Description[..150] + "..." : band.Description)}");
            promptBuilder.AppendLine();
        }

        promptBuilder.AppendLine("RAG CONTEXT DATA:");
        promptBuilder.AppendLine(ragContext);
        promptBuilder.AppendLine();

        promptBuilder.AppendLine("INSTRUCTIONS:");
        promptBuilder.AppendLine("1. Analyze the musical patterns in the vector search results");
        promptBuilder.AppendLine("2. Use the RAG context to understand broader musical relationships");
        promptBuilder.AppendLine("3. Suggest 10-15 underground/lesser-known bands that would complement these results");
        promptBuilder.AppendLine("4. Prioritize artists from the user's location when possible");
        promptBuilder.AppendLine("5. Focus on artists NOT already in the vector results");
        promptBuilder.AppendLine("6. Consider genre evolution and cross-pollination");
        promptBuilder.AppendLine();

        promptBuilder.AppendLine("RESPONSE FORMAT:");
        promptBuilder.AppendLine("Provide the band names in a simple list format.");

        return promptBuilder.ToString();
    }

    string IEnhancedAIPromptService.BuildAdvancedRecommendationPrompt(
        List<string> topArtists,
        List<string> genres,
        string location,
        List<BandModel> vectorResults,
        string ragContext)
    {
        var prompt = new StringBuilder();

        prompt.AppendLine("You are an expert music curator with deep knowledge of underground and emerging artists.");
        prompt.AppendLine($"User Location: {location}");
        prompt.AppendLine($"User's Top Artists: {string.Join(", ", topArtists)}");
        prompt.AppendLine($"User's Preferred Genres: {string.Join(", ", genres)}");
        prompt.AppendLine();

        prompt.AppendLine("Vector Search Results (Similar Artists Found):");
        foreach (var band in vectorResults.Take(8))
        {
            prompt.AppendLine($"- {band.Name} ({string.Join(", ", band.Genres)}) from {band.Location}");
        }

        prompt.AppendLine();
        prompt.AppendLine("RAG Context Data:");
        prompt.AppendLine(ragContext);
        prompt.AppendLine();

        prompt.AppendLine("Based on the vector search results and context data, suggest 8-12 additional underground artists that would perfectly complement these recommendations.");
        prompt.AppendLine("Focus on:");
        prompt.AppendLine("1. Artists not already in the vector search results");
        prompt.AppendLine("2. Lesser-known bands that match the user's taste profile");
        prompt.AppendLine("3. Local artists from the user's location when possible");
        prompt.AppendLine("4. Artists that bridge different genres in interesting ways");
        prompt.AppendLine();
        prompt.AppendLine("Provide ONLY the artist names in a numbered list format, one per line:");

        return prompt.ToString();
    }

    string IEnhancedAIPromptService.BuildRankingPrompt(
        List<BandModel> candidates,
        List<string> topArtists,
        List<string> genres,
        string location,
        int limit)
    {
        var prompt = new StringBuilder();

        prompt.AppendLine("You are a music recommendation expert. Rank the following artists in order of relevance for this user:");
        prompt.AppendLine($"User's Top Artists: {string.Join(", ", topArtists)}");
        prompt.AppendLine($"User's Genres: {string.Join(", ", genres)}");
        prompt.AppendLine($"User Location: {location}");
        prompt.AppendLine();

        prompt.AppendLine("Candidate Artists:");
        foreach (var band in candidates)
        {
            prompt.AppendLine($"- {band.Name} ({string.Join(", ", band.Genres)}) from {band.Location}");
        }

        prompt.AppendLine();
        prompt.AppendLine($"Rank these artists 1-{Math.Min(candidates.Count, limit)} based on:");
        prompt.AppendLine("1. Genre compatibility with user preferences");
        prompt.AppendLine("2. Geographic relevance (local artists preferred)");
        prompt.AppendLine("3. Musical similarity to user's top artists");
        prompt.AppendLine("4. Discovery potential (underground vs mainstream)");
        prompt.AppendLine();
        prompt.AppendLine("Provide ONLY the ranked artist names in order, one per line:");

        return prompt.ToString();
    }

    private static string GetPopularityDescription(double? popularity)
    {
        return popularity switch
        {
            null => "Underground",
            < 20 => "Deep Underground",
            < 40 => "Emerging",
            < 60 => "Growing Scene",
            < 80 => "Well-Known",
            _ => "Mainstream"
        };
    }
}
