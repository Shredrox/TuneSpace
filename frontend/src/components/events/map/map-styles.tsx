"use client";

import { createGlobalStyle } from "styled-components";

export const MapStyles = createGlobalStyle`
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
