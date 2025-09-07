export interface Trade {
    id?: number;
    symbol: string;
    accountId?: number;
    quantity: number;
    openDate: string;   // ISO date string (YYYY-MM-DD)
    openPrice: number;
    closeDate?: string | null;
    closePrice?: number | null;
  }
  
  export interface Position {
    symbol: string;
    quantity: number;
    avgCost: number;
    marketPrice?: number | null;
    marketValue?: number | null;
    unrealizedPnL: number;
  }

  export interface User {
    id: number;
    name: string;
  }

  export interface Account {
    id: number;
    name: string;
  }
