export interface Beat {
    id: number;
    title: string;
    bpm: number;
    key: string;
    audioUrl: string;
    leasePrice?: number;
    producerName: string;
    // We will add Auction properties later
}