export default interface Event {
  id: string;
  name: string;
  description: string;
  date: string;
  time: string;
  venue: string;
  address: string;
  city: string;
  country: string;
  coordinates: {
    latitude: number;
    longitude: number;
  };
  bandId: string;
  bandName: string;
  imageUrl?: string;
  ticketPrice?: number;
  ticketUrl?: string;
}
