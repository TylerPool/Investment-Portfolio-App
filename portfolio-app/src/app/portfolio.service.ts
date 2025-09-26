import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, of, map } from 'rxjs';
import { Trade, Position, Account, Portfolio } from './models';

@Injectable({ providedIn: 'root' })
export class PortfolioService {
  private http = inject(HttpClient);
  private base = 'http://localhost:5200/api';

  // getTrades(): Observable<Trade[]> {
  //   return this.http.get<Trade[]>(`${this.base}/trades`);
  // }

  // getTradesByAccount(accountId: number): Observable<Trade[]> {
  //   return this.http.get<Trade[]>(`${this.base}/accounts/${accountId}/trades`);
  // }

  getQuote(symbol: string) {
    return this.http.get<{ symbol: string; price: number }>(`${this.base}/quotes/${symbol}`);
  }

  getAccount(id: string): Observable<Account> {
    return this.http.get<Account>(`${this.base}/account/${id}`);
  }

  saveAccount(account: Account): Observable<Account> {
    return this.http
      .post<void>(`${this.base}/account`, account)
      .pipe(map(() => account));
  }

}
