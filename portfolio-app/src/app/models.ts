  export interface Portfolio {
    id: string
    accounts?: Account[];
  }
  
  export interface Account {
    id: string;
    name: string;
    trades?: Trade[];
  }

  export interface Trade {
    id?: number;
    symbol: string;
    accountId?: string;
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
