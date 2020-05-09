export interface Trade {
  id: number;
  symbol: string;
  direction: string;
  openDateTime: Date;
  pxOpen: number;
  closeDateTime: Date;
  pxClose: number;
}
