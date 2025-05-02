import React from "react";
import {
  Card,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "../shadcn/card";
import { Button } from "../shadcn/button";
import Event from "@/interfaces/Event";
import { CalendarIcon, MapPinIcon, ClockIcon, TicketIcon } from "lucide-react";
import { formatDate2 } from "@/utils/helpers";

interface EventCardProps {
  event: Event;
  isSelected?: boolean;
  variant?: "full" | "compact";
}

const EventCard: React.FC<EventCardProps> = ({
  event,
  isSelected,
  variant = "full",
}) => {
  return (
    <Card
      className={`w-full transition-all duration-300 hover:shadow-md 
        ${
          isSelected
            ? "border-2 border-primary shadow-md"
            : "border border-border/40"
        }`}
    >
      <CardHeader className="pb-2">
        <div className="flex justify-between items-start">
          <CardTitle className="text-lg font-bold line-clamp-1">
            {event.name}
          </CardTitle>
          {event.bandName && (
            <span className="text-xs bg-secondary px-2 py-1 rounded-full">
              {event.bandName}
            </span>
          )}
        </div>
        {variant === "full" && (
          <CardDescription className="line-clamp-2">
            {event.description}
          </CardDescription>
        )}
      </CardHeader>
      <CardContent className="pb-2">
        <div className="flex flex-col space-y-2 text-sm">
          <div className="flex items-center gap-2">
            <CalendarIcon size={16} className="text-primary" />
            <span>{formatDate2(event.date)}</span>
          </div>
          <div className="flex items-center gap-2">
            <ClockIcon size={16} className="text-primary" />
            <span>{event.time}</span>
          </div>
          <div className="flex items-center gap-2">
            <MapPinIcon size={16} className="text-primary" />
            <span className="line-clamp-1">
              {event.venue}, {event.city}, {event.country}
            </span>
          </div>
          {variant === "full" && event.ticketPrice && (
            <div className="flex items-center gap-2">
              <TicketIcon size={16} className="text-primary" />
              <span>${event.ticketPrice.toFixed(2)}</span>
            </div>
          )}
        </div>
      </CardContent>
      {(variant === "full" || isSelected) && event.ticketUrl && (
        <CardFooter>
          <Button className="w-full" variant="secondary" asChild>
            <a href={event.ticketUrl} target="_blank" rel="noopener noreferrer">
              Buy Tickets
            </a>
          </Button>
        </CardFooter>
      )}
    </Card>
  );
};

export default EventCard;
