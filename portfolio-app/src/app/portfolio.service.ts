import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of, map } from 'rxjs';
import { Trade, Position, Account, Portfolio } from './models';

@Injectable({ providedIn: 'root' })
export class PortfolioService {
  private http = inject(HttpClient);
  private base = 'http://localhost:5200/api';

  getPositions(): Observable<Position[]> {
    return this.http.get<Position[]>(`${this.base}/positions`);
  }

  getTrades(): Observable<Trade[]> {
    return this.http.get<Trade[]>(`${this.base}/trades`);
  }

  // New: trades scoped to an account
  getTradesByAccount(accountId: number): Observable<Trade[]> {
    return this.http.get<Trade[]>(`${this.base}/accounts/${accountId}/trades`);
  }

  addTrade(t: Trade): Observable<Trade> {
    return this.http.post<Trade>(`${this.base}/trades`, t);
  }

  deleteTrade(id: number) {
    return this.http.delete(`${this.base}/trades/${id}`);
  }

  getQuote(symbol: string) {
    return this.http.get<{ symbol: string; price: number }>(`${this.base}/quotes/${symbol}`);
  }

  getAccounts(): Observable<Account[]> {
    // Dummy accounts for now
    const accounts: Account[] = [
      { id: 1, name: 'Retirement Account' },
      { id: 2, name: 'Brokerage Account' }
    ];
    return of(accounts);
  }

  getPortfolio(): Observable<Portfolio> {
    return this.getAccounts().pipe(
      map((accounts) => ({ id: 'P-001', accounts }))
    );
  }
}
