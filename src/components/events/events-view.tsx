"use client";

import React, { useState } from "react";
import useEvents from "@/hooks/query/useEvents";
import MapView from "./map-view";
import EventsList from "./events-list";
import Event from "@/interfaces/Event";
import { Tabs, TabsContent, TabsList, TabsTrigger } from "../shadcn/tabs";
import { MapIcon, ListIcon, SearchIcon } from "lucide-react";
import { Input } from "../shadcn/input";

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
    address: "45 Symphony Road",
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
    address: "100 River Way",
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
    address: "15 Camden Street",
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
  // Bulgarian events with more detailed descriptions and locations
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

const EventsView: React.FC = () => {
  const { events: apiEvents, isLoading, isError, error } = useEvents();
  const [selectedEvent, setSelectedEvent] = useState<Event | null>(null);
  const [searchQuery, setSearchQuery] = useState("");
  const [viewMode, setViewMode] = useState<"map" | "list">("map");

  const events = apiEvents?.length ? apiEvents : mockEvents;

  const handleEventSelect = (event: Event) => {
    setSelectedEvent(event);
  };

  const handleEventClick = (eventId: string) => {
    const event = events?.find((e) => e.id === eventId) || null;
    if (event) {
      setSelectedEvent(event);
    }
  };

  const filteredEvents = events?.filter((event) => {
    if (!searchQuery) return true;

    const query = searchQuery.toLowerCase();
    return (
      event.name.toLowerCase().includes(query) ||
      (event.bandName && event.bandName.toLowerCase().includes(query)) ||
      (event.city && event.city.toLowerCase().includes(query)) ||
      (event.venue && event.venue.toLowerCase().includes(query)) ||
      (event.country && event.country.toLowerCase().includes(query))
    );
  });

  return (
    <div className="container mx-auto py-8 px-4">
      <div className="flex flex-col gap-6">
        <div className="flex flex-col md:flex-row md:justify-between md:items-center gap-4">
          <div>
            <h1 className="text-3xl font-bold mb-2 bg-clip-text text-transparent bg-gradient-to-r from-primary to-primary/70">
              Upcoming Events
            </h1>
            <p className="text-muted-foreground">
              Discover live performances from your favorite artists around the
              world
            </p>
          </div>

          <div className="flex items-center gap-2 max-w-md w-full">
            <div className="relative flex-1">
              <SearchIcon
                size={18}
                className="absolute left-3 top-1/2 transform -translate-y-1/2 text-muted-foreground"
              />
              <Input
                placeholder="Search events, bands, locations..."
                className="pl-10"
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
              />
            </div>
            <Tabs
              defaultValue="map"
              className="w-auto"
              onValueChange={(v) => setViewMode(v as "map" | "list")}
            >
              <TabsList>
                <TabsTrigger value="map">
                  <MapIcon size={18} />
                </TabsTrigger>
                <TabsTrigger value="list">
                  <ListIcon size={18} />
                </TabsTrigger>
              </TabsList>
            </Tabs>
          </div>
        </div>

        <div className="grid grid-cols-1 gap-6">
          {viewMode === "map" && (
            <div className="grid grid-cols-1 lg:grid-cols-4 gap-6">
              <div className="lg:col-span-3">
                <MapView
                  events={filteredEvents || []}
                  selectedEventId={selectedEvent?.id}
                  onEventClick={handleEventClick}
                />
              </div>
              <div className="lg:col-span-1">
                <EventsList
                  events={filteredEvents || []}
                  isLoading={isLoading && !filteredEvents.length}
                  error={error}
                  onEventSelect={handleEventSelect}
                  selectedEventId={selectedEvent?.id}
                />
              </div>
            </div>
          )}

          {viewMode === "list" && (
            <EventsList
              events={filteredEvents || []}
              isLoading={isLoading && !filteredEvents.length}
              error={error}
              onEventSelect={handleEventSelect}
              selectedEventId={selectedEvent?.id}
              displayMode="grid"
            />
          )}
        </div>
      </div>
    </div>
  );
};

export default EventsView;
