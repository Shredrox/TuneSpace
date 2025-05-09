"use client";

import { useState, useEffect } from "react";
import { useMap } from "react-leaflet";
import L from "leaflet";
import "leaflet.markercluster";
import MusicEvent from "@/interfaces/MusicEvent";
import {
  parseCoordinates,
  eventIcon,
  selectedEventIcon,
  formatDate,
} from "./map-utils";
import { extractTimeFromDate } from "@/utils/helpers";

interface MarkerClusterProps {
  events: MusicEvent[];
  selectedEventId?: string;
  onEventClick?: (eventId: string) => void;
}

const MarkerCluster = ({
  events,
  selectedEventId,
  onEventClick,
}: MarkerClusterProps) => {
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
      const coordinates = parseCoordinates(event.location);
      if (!coordinates) return;

      const isSelected = event.id === selectedEventId;

      const popupContent = `
        <div class="event-popup ${isDarkMode ? "dark-theme" : "light-theme"}">
          <h3 class="event-popup-title">${event.title}</h3>
          <div class="event-popup-band">${event.bandName}</div>
          <div class="event-popup-description">${event.description || ""}</div>
          <div class="event-popup-details">
            <div><strong>Date:</strong> ${formatDate(event.date)}</div>
            <div><strong>Time:</strong> ${extractTimeFromDate(event.date)}</div>
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

      const marker = L.marker([coordinates[0], coordinates[1]], {
        icon: isSelected ? selectedEventIcon : eventIcon,
      }).bindPopup(popupContent, {
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
        const coordinates = parseCoordinates(selectedEvent.location);
        if (coordinates) {
          map.setView(coordinates, 15);

          setTimeout(() => {
            markers.getLayers().forEach((layer: any) => {
              const layerPosition = layer.getLatLng();
              if (
                layerPosition.lat === coordinates[0] &&
                layerPosition.lng === coordinates[1]
              ) {
                layer.openPopup();
              }
            });
          }, 300);
        }
      }
    }

    return () => {
      map.removeLayer(markers);
    };
  }, [events, map, selectedEventId, onEventClick, isDarkMode]);

  return null;
};

export default MarkerCluster;
