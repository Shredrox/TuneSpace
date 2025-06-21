<div align="center">

# 🎵 TuneSpace

### _Discover the undiscovered. Connect with the underground._

<img src="https://img.shields.io/badge/.NET-9.0-512BD4?style=for-the-badge&logo=dotnet&logoColor=white" alt=".NET 9" />
<img src="https://img.shields.io/badge/Next.js-15-000000?style=for-the-badge&logo=nextdotjs&logoColor=white" alt="Next.js 15" />
<img src="https://img.shields.io/badge/React-19-61DAFB?style=for-the-badge&logo=react&logoColor=black" alt="React 19" />
<img src="https://img.shields.io/badge/TypeScript-5.0-3178C6?style=for-the-badge&logo=typescript&logoColor=white" alt="TypeScript" />
<img src="https://img.shields.io/badge/PostgreSQL-16-336791?style=for-the-badge&logo=postgresql&logoColor=white" alt="PostgreSQL" />

---

**🎤 For Artists** • Showcase your music • Connect with fans • Manage events & merchandise

**🎧 For Listeners** • Discover hidden gems • AI-powered recommendations • Underground music exploration

---

</div>

## 🌟 What is TuneSpace?

A comprehensive music discovery platform designed to connect **independent underground artists** with **listeners seeking fresh, undiscovered talent**. TuneSpace bridges the gap between emerging musicians and music enthusiasts through intelligent discovery algorithms and vibrant community features.

## 🎯 Mission

**For Artists:** Provide independent underground artists with a powerful platform to register, showcase their music, and gain exposure to targeted audiences who appreciate emerging talent.

**For Listeners:** Offer music enthusiasts an advanced discovery engine to find hidden gems and underground artists based on their listening preferences and location.

## ✨ Key Features

### 🎤 Artist/Band Management

- **Band Registration & Profiles**: Complete artist profiles with bio, genre classification, location, and cover images
- **Member Management**: Add and manage band members with role assignments
- **Event Management**: Manage music events with venue details and ticketing
- **Merchandise Integration**: Showcase and manage band merchandise
- **YouTube Integration**: Embed music videos and performances

### 🔍 Intelligent Music Discovery

- **AI-Powered Recommendations**: Advanced recommendation engine using multiple algorithms:
  - Standard RAG (Retrieval Augmented Generation) AI
  - Enhanced AI with adaptive learning
  - Collaborative filtering based on user similarity
  - Location-based discovery
- **Multi-Source Discovery**: Integrates with:
  - Spotify API
  - Bandcamp API
  - MusicBrainz
  - Last.fm
- **Hipster & Underground Search**: Specialized algorithms to find emerging and lesser-known artists
- **Genre Analysis**: Automatic genre enrichment based on listening history

### 🎧 Spotify Integration

- **OAuth Authentication**: Secure Spotify account linking
- **Listening History Analysis**: Analyze recently played tracks, followed artists, and top artists
- **Personalized Recommendations**: Leverage Spotify data for enhanced personalization
- **Profile Synchronization**: Import Spotify profile data and listening statistics

### 💬 Social & Community Features

- **Forums System**:
  - Category-based discussions
  - Threaded conversations with nested replies
  - Like/unlike functionality
  - Music and event sharing cards
  - Role-based badges
- **Real-time Chat**:
  - Private messaging between users
  - Band-to-fan direct messaging
  - SignalR-powered real-time communication
- **Band Following**: Follow favorite artists and get updates
- **User Search**: Find and connect with other music enthusiasts

### 🗺️ Location-Based Features

- **Geographic Discovery**: Find local artists and events in your area
- **Venue Mapping**: Interactive maps for event locations using Leaflet
- **IP-based Location Detection**: Automatic location detection for relevant recommendations

### 🔐 Authentication & Security

- **JWT-based Authentication**: Secure token-based user authentication
- **ASP.NET Core Identity**: Comprehensive user management with roles
- **OAuth Integration**: Spotify external authentication
- **Cookie-based Session Management**: Secure session handling

## 🏗️ Technical Architecture

### Backend (.NET 9)

- **Clean Architecture**: Organized into Core, Application, Infrastructure, and API layers
- **Entity Framework Core**: Database ORM with PostgreSQL support
- **SignalR**: Real-time communication for chat and notifications

### Frontend (Next.js 15)

- **React 19**: Latest React features with concurrent rendering
- **TypeScript**: Full type safety throughout the application
- **Tailwind CSS**: Utility-first styling with custom design system
- **shadcn/ui**: Modern, accessible component library
- **TanStack Query**: Powerful data fetching and caching
- **React Hook Form**: Efficient form management with validation
- **Leaflet**: Interactive maps for location features
- **Date-fns**: Comprehensive date manipulation

### External Integrations

- **Spotify Web API**: Artist data, user listening history, and authentication
- **Bandcamp API**: Underground artist discovery
- **MusicBrainz API**: Comprehensive music metadata
- **Last.fm API**: Similar artists and enhanced metadata
- **Countries API**: Location data for geographical features

## 📁 Project Structure

```
TuneSpace/
├── backend/
│   ├── TuneSpace.Api/          # Web API controllers and configuration
│   ├── TuneSpace.Application/  # Business logic and services
│   ├── TuneSpace.Core/         # Domain entities and interfaces
│   ├── TuneSpace.Infrastructure/ # Data access and external services
│   └── TuneSpace.Tests/        # Unit and integration tests
└── frontend/
    ├── src/
    │   ├── app/                # Next.js app router pages
    │   ├── components/         # React components
    │   ├── hooks/              # Custom React hooks
    │   ├── interfaces/         # TypeScript type definitions
    │   ├── services/           # API service layer
    │   └── utils/              # Utility functions
    └── public/                 # Static assets
```

## 🚀 Getting Started

### Prerequisites

- **Backend**: .NET 9 SDK, PostgreSQL
- **Frontend**: Node.js 20+, npm/yarn
- **External APIs**: Spotify Developer Account

### Backend Setup

1. Navigate to the backend directory:
   ```bash
   cd backend
   ```
2. Restore dependencies:
   ```bash
   dotnet restore
   ```
3. Update database connection string in `appsettings.json`
4. Run database migrations:
   ```bash
   dotnet ef database update
   ```
5. Configure Spotify API credentials in user secrets:
   ```bash
   dotnet user-secrets set "Spotify:ClientId" "your-client-id"
   dotnet user-secrets set "Spotify:ClientSecret" "your-client-secret"
   ```
6. Start the API:
   ```bash
   dotnet run --project TuneSpace.Api
   ```

### Frontend Setup

1. Navigate to the frontend directory:
   ```bash
   cd frontend
   ```
2. Install dependencies:
   ```bash
   npm install
   ```
3. Start the development server:
   ```bash
   npm run dev
   ```

## 🎵 Core Discovery Algorithm

TuneSpace employs a sophisticated multi-stage recommendation engine:

### 1. Data Collection

- Spotify listening history (recently played, followed artists, top artists)
- User location and preferences
- Genre analysis and enrichment

### 2. Multi-Source Aggregation

- **Local Artists**: MusicBrainz + Bandcamp by location
- **Registered Bands**: Internal database with enhanced metadata
- **Underground Discovery**: Spotify search with specialized queries
- **AI Recommendations**: RAG or Enhanced AI with confidence scoring
- **Collaborative Filtering**: User similarity matching

### 3. Data Enrichment

- Last.fm integration for similar artists and metadata
- Genre classification and expansion
- Popularity and relevance scoring

### 4. Adaptive Scoring

- Machine learning-based scoring with user feedback
- Confidence boosting for high-quality matches
- Location-based relevance adjustments
- Diversity algorithms to prevent echo chambers

### 5. Final Processing

- Deduplication and cooldown management
- Relevance ranking and recommendation limits
- Real-time caching for performance

## 🗃️ Database Schema

### Core Entities

- **Users**: Identity-based user management with roles
- **Bands**: Artist profiles with metadata and relationships
- **Music Events**: Event management with location and ticketing
- **Forum System**: Categories, threads, posts, and likes
- **Chat System**: Private messaging and band communication
- **Follows**: User-to-band relationship tracking

### Key Relationships

- Users can be members of multiple bands
- Bands can have multiple events and merchandise
- Forum threads support nested post structures
- Chat system supports both user-to-user and band-to-user communication

## 🔮 Future Roadmap

- **Music Streaming**: Direct music playback integration
- **Advanced Analytics**: Artist performance insights and listener analytics
- **Recommendation Learning**: Enhanced machine learning with user feedback loops
- **Monetization**: Artist promotion tools, ticket selling and premium features

---

**TuneSpace** - _Discover the undiscovered. Connect with the underground._
