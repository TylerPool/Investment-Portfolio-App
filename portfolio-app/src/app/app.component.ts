import { Component, OnInit, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { PortfolioService } from './portfolio.service';
import { Position, Trade, User, Account, Portfolio } from './models';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  private svc = inject(PortfolioService);

  user = signal<User>({
    id: 1234,
    name: 'Alice Smith'
  });  
  portfolio = signal<Portfolio>({id: "P-001"});
  accounts = signal<Account[]>([]);
  selectedAccountId = signal<number | null>(null);

  positions = signal<Position[]>([]);
  trades = signal<Trade[]>([]);
  
  loading = signal(false);
  error = signal<string | null>(null);


  newTrade: Trade = {
    symbol: 'AAPL',
    quantity: 10,
    openDate: new Date().toISOString().slice(0,10),
    openPrice: 100
  };

  ngOnInit() {
    this.loadAccounts();
    this.loadPortfolioById("P-001");
    this.refresh();
  }

  loadAccounts() {
    this.svc.getAccounts().subscribe({
      next: (accts) => {
        this.accounts.set(accts);
        // default to first account if none selected yet
        if (accts.length && this.selectedAccountId() == null) {
          this.selectedAccountId.set(accts[0].id);
          // load trades for default account
          this.loadTradesForSelectedAccount();
        }
      },
      error: (e) => this.error.set(e.message)
    });
  }

  onAccountChanged(val: string | number) {
    // <select> emits strings; coerce to number for safety
    const id = typeof val === 'string' ? parseInt(val, 10) : val;
    this.selectedAccountId.set(id);
    // Load trades specific to the selected account
    this.loadTradesForSelectedAccount();
  }

  refresh() {
    this.loading.set(true);
    this.svc.getPositions().subscribe({
      next: pos => this.positions.set(pos),
      error: e => this.error.set(e.message)
    });

    const accountId = this.selectedAccountId();
    const trades$ = accountId != null
      ? this.svc.getTradesByAccount(accountId)
      : this.svc.getTrades();

    trades$.subscribe({
      next: ts => this.trades.set(ts),
      error: e => this.error.set(e.message),
      complete: () => this.loading.set(false)
    });
  }

  addTrade() {
    this.error.set(null);
    const t: Trade = {
      ...this.newTrade,
      symbol: this.newTrade.symbol.trim().toUpperCase(),
      accountId: this.selectedAccountId() ?? undefined
    };
    this.svc.addTrade(t).subscribe({
      next: _ => {
        this.newTrade = { symbol: 'AAPL', quantity: 10, openDate: new Date().toISOString().slice(0,10), openPrice: 100 };
        this.refresh();
      },
      error: e => this.error.set(e.message)
    });
  }

  removeTrade(id?: number) {
    if (!id) return;
    this.svc.deleteTrade(id).subscribe({ next: () => this.refresh() });
  }

  private loadTradesForSelectedAccount() {
    const id = this.selectedAccountId();
    if (id == null) return;
    this.svc.getTradesByAccount(id).subscribe({
      next: ts => this.trades.set(ts),
      error: e => this.error.set(e.message)
    });
  }

  private loadPortfolioById(id: string){
    this.svc.getPortfolio().subscribe({
      next: (p) => this.portfolio.set(p),
      error: (e) => this.error.set(e.message)
    });
  }
}
