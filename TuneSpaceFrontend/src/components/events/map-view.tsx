"use client";

import React, { useState, useEffect, useRef } from "react";
import {
  MapContainer,
  TileLayer,
  Marker,
  Popup,
  useMap,
  ZoomControl,
} from "react-leaflet";
import L from "leaflet";
import "leaflet/dist/leaflet.css";
import "leaflet.markercluster/dist/MarkerCluster.css";
import "leaflet.markercluster/dist/MarkerCluster.Default.css";
import "leaflet.markercluster";
import Event from "@/interfaces/Event";

const mockEvents: Event[] = [
  {
    id: "1",
    name: "Rock Festival 2023",
    description: "The biggest rock event of the year featuring top bands",
    bandName: "Multiple Artists",
    bandId: "b1",
    venue: "Central Park",
    address: "Central Park West",
    city: "New York",
    country: "USA",
    date: "2023-12-15",
    time: "16:00",
    coordinates: {
      latitude: 40.785091,
      longitude: -73.968285,
    },
    ticketPrice: 75,
    ticketUrl: "https://example.com/tickets/1",
    imageUrl: "https://example.com/images/rockfest.jpg",
  },
  {
    id: "2",
    name: "Jazz Night",
    description: "An evening of smooth jazz and blues",
    bandName: "Blue Note Quartet",
    bandId: "b2",
    venue: "The Blue Room",
    address: "123 Jazz Street",
    city: "Chicago",
    country: "USA",
    date: "2023-12-18",
    time: "20:00",
    coordinates: {
      latitude: 41.878113,
      longitude: -87.629799,
    },
    ticketPrice: 45,
    ticketUrl: "https://example.com/tickets/2",
    imageUrl: "https://example.com/images/jazznight.jpg",
  },
  {
    id: "3",
    name: "Electronic Music Experience",
    description: "Immersive electronic music with light show",
    bandName: "DJ Pulse",
    bandId: "b3",
    venue: "Warehouse 23",
    address: "23 Techno Boulevard",
    city: "Berlin",
    country: "Germany",
    date: "2023-12-22",
    time: "22:00",
    coordinates: {
      latitude: 52.520008,
      longitude: 13.404954,
    },
    ticketPrice: 60,
    ticketUrl: "https://example.com/tickets/3",
    imageUrl: "https://example.com/images/electronic.jpg",
  },
  {
    id: "4",
    name: "Classical Symphony",
    description: "Beethoven's 9th performed by city orchestra",
    bandName: "Metropolitan Orchestra",
    bandId: "b4",
    venue: "Concert Hall",
    address: "1 Symphony Road",
    city: "Vienna",
    country: "Austria",
    date: "2023-12-25",
    time: "19:30",
    coordinates: {
      latitude: 48.210033,
      longitude: 16.363449,
    },
    ticketPrice: 90,
    ticketUrl: "https://example.com/tickets/4",
    imageUrl: "https://example.com/images/symphony.jpg",
  },
  {
    id: "5",
    name: "Country Music Festival",
    description: "Three days of country music and southern hospitality",
    bandName: "Multiple Artists",
    bandId: "b5",
    venue: "Riverfront Park",
    address: "456 Country Lane",
    city: "Nashville",
    country: "USA",
    date: "2023-12-28",
    time: "12:00",
    coordinates: {
      latitude: 36.174465,
      longitude: -86.76796,
    },
    ticketPrice: 65,
    ticketUrl: "https://example.com/tickets/5",
    imageUrl: "https://example.com/images/country.jpg",
  },
  {
    id: "6",
    name: "Indie Band Showcase",
    description: "Discover the next big indie artists",
    bandName: "Various Indie Bands",
    bandId: "b6",
    venue: "The Underground",
    address: "789 Indie Street",
    city: "London",
    country: "UK",
    date: "2024-01-05",
    time: "18:00",
    coordinates: {
      latitude: 51.507351,
      longitude: -0.127758,
    },
    ticketPrice: 30,
    ticketUrl: "https://example.com/tickets/6",
    imageUrl: "https://example.com/images/indie.jpg",
  },
  // Updated Bulgarian events
  {
    id: "7",
    name: "Sofia Music Festival 2023",
    description:
      "The biggest annual music festival in Bulgaria with international and local artists",
    bandName: "Various Artists",
    bandId: "b7",
    venue: "Borisova Garden",
    address: "Tsar Osvoboditel Blvd",
    city: "Sofia",
    country: "Bulgaria",
    date: "2023-12-10",
    time: "15:00",
    coordinates: {
      latitude: 42.6859,
      longitude: 23.3417,
    },
    ticketPrice: 40,
    ticketUrl: "https://example.com/tickets/7",
    imageUrl: "https://example.com/images/sofia-music.jpg",
  },
  {
    id: "8",
    name: "Varna Summer Jazz Festival",
    description:
      "Annual jazz event on the Black Sea coast featuring international and local jazz musicians",
    bandName: "Varna Jazz Ensemble",
    bandId: "b8",
    venue: "Seaside Stage",
    address: "Varna Beach Alley",
    city: "Varna",
    country: "Bulgaria",
    date: "2023-12-20",
    time: "19:30",
    coordinates: {
      latitude: 43.2073,
      longitude: 27.9245,
    },
    ticketPrice: 25,
    ticketUrl: "https://example.com/tickets/8",
    imageUrl: "https://example.com/images/varna-jazz.jpg",
  },
  {
    id: "9",
    name: "Ancient Theatre Live Rock",
    description:
      "Rock concert in the incredible ambiance of the Roman Amphitheatre in Plovdiv",
    bandName: "Thunder Road",
    bandId: "b9",
    venue: "Ancient Theatre of Philippopolis",
    address: "Old Town",
    city: "Plovdiv",
    country: "Bulgaria",
    date: "2023-12-23",
    time: "20:00",
    coordinates: {
      latitude: 42.1421,
      longitude: 24.7501,
    },
    ticketPrice: 35,
    ticketUrl: "https://example.com/tickets/9",
    imageUrl: "https://example.com/images/plovdiv-rock.jpg",
  },
  {
    id: "10",
    name: "Bulgarian Folklore Festival",
    description:
      "Traditional Bulgarian music, dancing and cultural performances",
    bandName: "National Folklore Ensemble",
    bandId: "b10",
    venue: "National Palace of Culture",
    address: "1 Bulgaria Blvd",
    city: "Sofia",
    country: "Bulgaria",
    date: "2023-12-30",
    time: "18:00",
    coordinates: {
      latitude: 42.6856,
      longitude: 23.3194,
    },
    ticketPrice: 20,
    ticketUrl: "https://example.com/tickets/10",
    imageUrl: "https://example.com/images/folk-music.jpg",
  },
  {
    id: "11",
    name: "Burgas Metal Fest",
    description:
      "Hardcore metal bands from across Eastern Europe gather for an epic night",
    bandName: "Multiple Metal Bands",
    bandId: "b11",
    venue: "Burgas Live Club",
    address: "Port Area",
    city: "Burgas",
    country: "Bulgaria",
    date: "2023-12-27",
    time: "21:00",
    coordinates: {
      latitude: 42.4953,
      longitude: 27.4717,
    },
    ticketPrice: 30,
    ticketUrl: "https://example.com/tickets/11",
    imageUrl: "https://example.com/images/burgas-metal.jpg",
  },
  {
    id: "12",
    name: "Rila Mountain Folk Music Retreat",
    description:
      "Traditional folk music in the serene setting of Bulgaria's highest mountain",
    bandName: "Mountain Echo Ensemble",
    bandId: "b12",
    venue: "Rila Monastery Area",
    address: "Rila Mountain",
    city: "Rila",
    country: "Bulgaria",
    date: "2024-01-05",
    time: "14:00",
    coordinates: {
      latitude: 42.1335,
      longitude: 23.3402,
    },
    ticketPrice: 15,
    ticketUrl: "https://example.com/tickets/12",
    imageUrl: "https://example.com/images/rila-folk.jpg",
  },
];

const createEventIcon = (isSelected = false) => {
  const svgIcon = `
    <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24" width="36" height="36">
      <circle cx="12" cy="12" r="10" fill="${
        isSelected ? "#f43f5e" : "#3B82F6"
      }" stroke="${isSelected ? "#be123c" : "#1E40AF"}" stroke-width="2" />
      <path d="M8,10 L8,14 M12,8 L12,16 M16,10 L16,14 M6.5,16.5 C6.5,16.5 8.5,14.5 12,14.5 C15.5,14.5 17.5,16.5 17.5,16.5" 
        stroke="white" stroke-width="1.5" stroke-linecap="round" fill="none" />
    </svg>
  `;

  return L.divIcon({
    html: svgIcon,
    className: isSelected ? "event-marker selected" : "event-marker",
    iconSize: [36, 36],
    iconAnchor: [18, 36],
    popupAnchor: [0, -36],
  });
};

const eventIcon = createEventIcon();
const selectedEventIcon = createEventIcon(true);

interface MapProps {
  events?: Event[];
  selectedEventId?: string;
  onEventClick?: (eventId: string) => void;
}

const MarkerCluster: React.FC<{
  events: Event[];
  selectedEventId?: string;
  onEventClick?: (eventId: string) => void;
}> = ({ events, selectedEventId, onEventClick }) => {
  const map = useMap();
  const [isDarkMode, setIsDarkMode] = useState(false);

  useEffect(() => {
    const checkDarkMode = () => {
      const isDarkMedia = window.matchMedia(
        "(prefers-color-scheme: dark)"
      ).matches;
      const isDarkClass = document.documentElement.classList.contains("dark");
      setIsDarkMode(isDarkMedia || isDarkClass);
    };

    checkDarkMode();

    const mediaQuery = window.matchMedia("(prefers-color-scheme: dark)");
    const handleChange = () => checkDarkMode();
    mediaQuery.addEventListener("change", handleChange);

    const observer = new MutationObserver(checkDarkMode);
    observer.observe(document.documentElement, {
      attributes: true,
      attributeFilter: ["class"],
    });

    return () => {
      mediaQuery.removeEventListener("change", handleChange);
      observer.disconnect();
    };
  }, []);

  useEffect(() => {
    const markers = L.markerClusterGroup({
      maxClusterRadius: 40,
      spiderfyOnMaxZoom: true,
      showCoverageOnHover: true,
      zoomToBoundsOnClick: true,
      animate: true,
      animateAddingMarkers: true,
      disableClusteringAtZoom: 17,
    });

    events.forEach((event) => {
      const isSelected = event.id === selectedEventId;

      const popupContent = `
        <div class="event-popup ${isDarkMode ? "dark-theme" : "light-theme"}">
          <h3 class="event-popup-title">${event.name}</h3>
          <div class="event-popup-band">${event.bandName}</div>
          <div class="event-popup-description">${event.description}</div>
          <div class="event-popup-details">
            <div><strong>Date:</strong> ${formatDate(event.date)}</div>
            <div><strong>Time:</strong> ${event.time}</div>
            <div><strong>Venue:</strong> ${event.venue}</div>
            <div><strong>Location:</strong> ${event.city}, ${
        event.country
      }</div>
            ${
              event.ticketPrice
                ? `<div><strong>Price:</strong> $${event.ticketPrice.toFixed(
                    2
                  )}</div>`
                : ""
            }
          </div>
          ${
            event.ticketUrl
              ? `<a href="${event.ticketUrl}" target="_blank" class="event-popup-button">Buy Tickets</a>`
              : ""
          }
        </div>
      `;

      const marker = L.marker(
        [event.coordinates.latitude, event.coordinates.longitude],
        {
          icon: isSelected ? selectedEventIcon : eventIcon,
        }
      ).bindPopup(popupContent, {
        maxWidth: 300,
        className: isDarkMode
          ? "event-custom-popup dark-popup"
          : "event-custom-popup light-popup",
      });

      marker.on("click", () => {
        if (onEventClick) {
          onEventClick(event.id);
        }
      });

      markers.addLayer(marker);
    });

    map.addLayer(markers);

    if (selectedEventId) {
      const selectedEvent = events.find((e) => e.id === selectedEventId);
      if (selectedEvent) {
        const position = [
          selectedEvent.coordinates.latitude,
          selectedEvent.coordinates.longitude,
        ];
        map.setView(position, 15);

        setTimeout(() => {
          markers.getLayers().forEach((layer: any) => {
            const layerPosition = layer.getLatLng();
            if (
              layerPosition.lat === position[0] &&
              layerPosition.lng === position[1]
            ) {
              layer.openPopup();
            }
          });
        }, 300);
      }
    }

    return () => {
      map.removeLayer(markers);
    };
  }, [events, map, selectedEventId, onEventClick, isDarkMode]);

  return null;
};

const FlyToLocation: React.FC<{
  lat: number;
  lng: number;
  zoom: number;
}> = ({ lat, lng, zoom }) => {
  const map = useMap();

  useEffect(() => {
    map.flyTo([lat, lng], zoom, {
      animate: true,
      duration: 1.5,
    });
  }, [lat, lng, zoom, map]);

  return null;
};

const FullscreenControl: React.FC = () => {
  const map = useMap();
  const [isFullscreen, setIsFullscreen] = useState(false);

  const toggleFullscreen = () => {
    const container = map.getContainer();
    if (!isFullscreen) {
      if (container.requestFullscreen) {
        container.requestFullscreen();
      }
    } else {
      if (document.exitFullscreen) {
        document.exitFullscreen();
      }
    }
    setIsFullscreen(!isFullscreen);
  };

  return (
    <div className="leaflet-top leaflet-left" style={{ marginTop: "60px" }}>
      <div className="leaflet-control leaflet-bar">
        <a
          href="#"
          onClick={(e) => {
            e.preventDefault();
            toggleFullscreen();
          }}
          title="Toggle fullscreen"
          role="button"
          style={{
            display: "flex",
            justifyContent: "center",
            alignItems: "center",
            width: "30px",
            height: "30px",
          }}
        >
          {isFullscreen ? "⤓" : "⤢"}
        </a>
      </div>
    </div>
  );
};

const MapView: React.FC<MapProps> = ({
  events = [],
  selectedEventId,
  onEventClick,
}) => {
  const mapRef = useRef<L.Map | null>(null);
  const [userLocation, setUserLocation] = useState<[number, number] | null>(
    null
  );
  const [mapStyle, setMapStyle] = useState<string>(
    "https://tiles.stadiamaps.com/tiles/alidade_smooth_dark/{z}/{x}/{y}{r}.png"
  );

  const displayEvents = events.length > 0 ? events : mockEvents;

  useEffect(() => {
    navigator.geolocation.getCurrentPosition(
      (position) => {
        setUserLocation([position.coords.latitude, position.coords.longitude]);
      },
      (error) => {
        console.warn(
          `Could not retrieve user location: ${error.message}. Using default location.`
        );
        if (displayEvents.length > 0) {
          setUserLocation([
            displayEvents[0].coordinates.latitude,
            displayEvents[0].coordinates.longitude,
          ]);
        } else {
          setUserLocation([51.505, -0.09]);
        }
      },
      { enableHighAccuracy: true, timeout: 5000, maximumAge: 0 }
    );

    const style = document.createElement("style");
    style.innerHTML = `
      .event-marker {
        filter: drop-shadow(0 0 5px rgba(0, 0, 0, 0.5));
        transition: transform 0.3s ease;
      }
      
      .event-marker:hover {
        transform: scale(1.1);
      }
      
      .event-marker.selected {
        transform: scale(1.2);
        z-index: 1000 !important;
      }
      
      .leaflet-container {
        border-radius: 12px;
        box-shadow: 0 8px 16px rgba(0, 0, 0, 0.1);
      }
      
      .map-style-switcher {
        position: absolute;
        bottom: 10px;
        right: 10px;
        z-index: 1000;
        background: white;
        padding: 5px;
        border-radius: 4px;
        box-shadow: 0 2px 5px rgba(0, 0, 0, 0.1);
      }
      
      /* Base popup styling */
      .event-custom-popup .leaflet-popup-content-wrapper {
        border-radius: 8px;
        padding: 0;
        overflow: hidden;
      }
      
      .event-custom-popup .leaflet-popup-content {
        margin: 0;
        width: 280px !important;
      }
      
      .event-popup {
        padding: 0;
        font-family: system-ui, -apple-system, sans-serif;
      }
      
      /* Light theme popup */
      .light-popup .leaflet-popup-content-wrapper, 
      .light-popup .leaflet-popup-tip {
        background: rgba(255, 255, 255, 0.95);
        color: hsl(20 14.3% 4.1%);
      }
      
      .light-theme .event-popup-title {
        background: hsl(24.6 95% 53.1%);
        color: hsl(60 9.1% 97.8%);
        margin: 0;
        padding: 10px 15px;
        font-size: 16px;
        font-weight: bold;
      }
      
      .light-theme .event-popup-band {
        background: hsl(25 95% 50%);
        color: white;
        padding: 5px 15px;
        font-size: 12px;
        font-style: italic;
      }
      
      .light-theme .event-popup-description {
        padding: 10px 15px;
        border-bottom: 1px solid hsl(20 5.9% 90%);
        font-size: 13px;
      }
      
      .light-theme .event-popup-details {
        padding: 10px 15px;
        font-size: 12px;
        line-height: 1.5;
      }
      
      .light-theme .event-popup-button {
        display: block;
        background: hsl(24.6 95% 53.1%);
        color: white;
        text-align: center;
        padding: 8px 15px;
        text-decoration: none;
        font-weight: bold;
        margin-top: 5px;
        transition: background-color 0.3s;
      }
      
      .light-theme .event-popup-button:hover {
        background: hsl(25 95% 50%);
      }
      
      /* Dark theme popup */
      .dark-popup .leaflet-popup-content-wrapper, 
      .dark-popup .leaflet-popup-tip {
        background: rgba(34, 37, 45, 0.95);
        color: hsl(213 31% 91%);
      }
      
      .dark-theme .event-popup-title {
        background: hsl(199 89% 48%);
        color: hsl(213 31% 91%);
        margin: 0;
        padding: 10px 15px;
        font-size: 16px;
        font-weight: bold;
      }
      
      .dark-theme .event-popup-band {
        background: hsl(210 89% 40%);
        color: white;
        padding: 5px 15px;
        font-size: 12px;
        font-style: italic;
      }
      
      .dark-theme .event-popup-description {
        padding: 10px 15px;
        border-bottom: 1px solid hsl(216 34% 17%);
        font-size: 13px;
      }
      
      .dark-theme .event-popup-details {
        padding: 10px 15px;
        font-size: 12px;
        line-height: 1.5;
      }
      
      .dark-theme .event-popup-details strong {
        color: hsl(199 89% 70%);
      }
      
      .dark-theme .event-popup-button {
        display: block;
        background: hsl(199 89% 48%);
        color: white;
        text-align: center;
        padding: 8px 15px;
        text-decoration: none;
        font-weight: bold;
        margin-top: 5px;
        transition: background-color 0.3s;
      }
      
      .dark-theme .event-popup-button:hover {
        background: hsl(210 89% 40%);
      }
    `;
    document.head.appendChild(style);

    return () => {
      document.head.removeChild(style);
    };
  }, [displayEvents]);

  const changeMapStyle = (style: string) => {
    setMapStyle(style);
  };

  const defaultLocation: [number, number] = [40.7128, -74.006];

  const [useLocationFilter, setUseLocationFilter] = useState(false);
  const [filterRadius, setFilterRadius] = useState(50); // in km
  const [selectedCountry, setSelectedCountry] = useState<string | null>(null);

  const filteredEvents =
    useLocationFilter && selectedCountry
      ? displayEvents.filter((event) => event.country === selectedCountry)
      : displayEvents;

  const LocationFilterControl: React.FC = () => {
    const map = useMap();

    const handleLocationToggle = () => {
      setUseLocationFilter(!useLocationFilter);

      if (!useLocationFilter) {
        setSelectedCountry("Bulgaria");

        const bulgarianEvents = displayEvents.filter(
          (e) => e.country === "Bulgaria"
        );
        if (bulgarianEvents.length > 0) {
          const center = [42.7249, 25.4827];
          map.setView(center as [number, number], 7);
        }
      }
    };

    return (
      <div
        className="leaflet-top leaflet-right"
        style={{ zIndex: 1000, marginTop: "10px", marginRight: "10px" }}
      >
        <div className="leaflet-control p-2 bg-card rounded-lg shadow-sm border border-border/40">
          <div className="flex items-center gap-2">
            <span className="text-sm text-muted-foreground">
              {useLocationFilter ? "Location: Bulgaria" : "Location: All"}
            </span>
            <button
              onClick={handleLocationToggle}
              className="flex items-center"
              aria-label="Toggle location-based filtering"
            >
              <div
                className={`w-10 h-5 rounded-full p-0.5 transition-colors ${
                  useLocationFilter ? "bg-primary" : "bg-muted"
                }`}
              >
                <div
                  className={`w-4 h-4 rounded-full bg-white transition-transform transform ${
                    useLocationFilter ? "translate-x-5" : "translate-x-0"
                  }`}
                />
              </div>
            </button>
          </div>
        </div>
      </div>
    );
  };

  return (
    <div
      className="map-container w-full h-full relative"
      style={{ height: "70vh", borderRadius: "12px", overflow: "hidden" }}
    >
      {(userLocation || filteredEvents.length > 0) && (
        <MapContainer
          center={userLocation || defaultLocation}
          zoom={13}
          style={{ height: "100%", width: "100%" }}
          ref={(ref) => {
            if (ref) {
              mapRef.current = ref;
            }
          }}
          zoomControl={false}
        >
          <TileLayer
            url={mapStyle}
            attribution='&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors | Map tiles by <a href="https://stadiamaps.com/" target="_blank">Stadia Maps</a>'
          />
          <ZoomControl position="bottomleft" />
          <FullscreenControl />
          <LocationFilterControl />
          <MarkerCluster
            events={filteredEvents}
            selectedEventId={selectedEventId}
            onEventClick={onEventClick}
          />

          <div className="map-style-switcher">
            <select
              onChange={(e) => changeMapStyle(e.target.value)}
              className="text-sm p-1 border rounded"
              defaultValue="https://tiles.stadiamaps.com/tiles/alidade_smooth_dark/{z}/{x}/{y}{r}.png"
            >
              <option value="https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png">
                Standard
              </option>
              <option value="https://tiles.stadiamaps.com/tiles/alidade_smooth_dark/{z}/{x}/{y}{r}.png">
                Dark
              </option>
              <option value="https://server.arcgisonline.com/ArcGIS/rest/services/World_Imagery/MapServer/tile/{z}/{y}/{x}">
                Satellite
              </option>
              <option value="https://tiles.stadiamaps.com/tiles/alidade_smooth/{z}/{x}/{y}{r}.png">
                Light
              </option>
            </select>
          </div>
        </MapContainer>
      )}
    </div>
  );
};

const formatDate = (dateString: string): string => {
  if (!dateString) return "";

  try {
    const date = new Date(dateString);
    return date.toLocaleDateString("en-US", {
      year: "numeric",
      month: "long",
      day: "numeric",
    });
  } catch (e) {
    return dateString;
  }
};

export default MapView;
