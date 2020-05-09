export interface Trade {
  id: number;
  symbol: string;
  direction: string;
  quantity: number;
  openDateTime: Date;
  pxOpen: number;
  closeDateTime: Date;
  pxClose: number;
}
