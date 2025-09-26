import { Component, OnInit, inject, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { forkJoin } from 'rxjs';
import { PortfolioService } from './portfolio.service';
import { Position, Trade, User, Account, Portfolio } from './models';
import { PortfolioSummary } from './portfolio-summary/portfolio-summary.component';
import { AccountUnrealizedComponent } from './account-unrealized.component/account-unrealized.component';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [CommonModule, FormsModule, PortfolioSummary, AccountUnrealizedComponent],
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  private svc = inject(PortfolioService);
  //for now populate with some dummy user and portfolio data.
  //later replace with login screen
  user = signal<User>({
    id: 1234,
    name: 'Alice Smith'
  });  
  portfolio = signal<Portfolio>({
    id: "P-001"
    });
  accounts = signal<Account[]>([]);
  selectedAccountId = signal<string | null>(null);
  selectedAccount = computed(() => {
    const id = this.selectedAccountId();
    if (id == null) {
      return null;
    }

    return this.accounts().find((account) => account.id === id) ?? null;
  });
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

  //#region App Methods
  ngOnInit() {
    // For now, load dummy portfolio by default
    this.loadPortfolio();
    this.refresh();
  }

  refresh() {
    this.loading.set(true);
    this.updatePositions();
    this.loading.set(false);
  }
  //#endregion
  
  //#region Portfolio and Account Methods
  private loadPortfolio(){
    forkJoin([
      this.svc.getAccount('1'),
      this.svc.getAccount('2')
    ]).subscribe({
      next: (accounts) => {
        this.accounts.set(accounts);
        this.portfolio.update((current) => ({
          ...current,
          accounts
        }));

        const parsedId = accounts[0]?.id != null ? accounts[0].id : null;
        this.onAccountChanged(parsedId);
      },
      error: (e) => {
        const message = e instanceof Error ? e.message : 'Failed to load accounts';
        this.error.set(message);
      }
    });
  }

  onAccountChanged(id: string | null) {
    this.selectedAccountId.set(id);
    this.updatePositions();
  }  

  saveSelectedAccount() {
    const accountId = this.selectedAccountId();
    if (accountId == null) {
      this.error.set('Please select an account before saving.');
      return;
    }

    const account = this.accounts().find(a => a.id === accountId);
    if (!account) {
      this.error.set('Selected account could not be found.');
      return;
    }

    this.svc.saveAccount(account).subscribe({
      next: () => {
        // no-op for now
      },
      error: (e) => this.error.set(e.message)
    });
  }

  //#endregion

  updatePositions(){
    const accountId = this.selectedAccountId();
    if (accountId == null) {
      this.positions.set([]);
      return;
    }

    const account = this.accounts().find(a => a.id === accountId);
    const trades = account?.trades ?? [];
    if (!trades.length) {
      this.positions.set([]);
      return;
    }

    const totals = new Map<string, number>();
    for (const trade of trades) {
      const symbol = trade.symbol?.trim().toUpperCase();
      if (!symbol) {
        continue;
      }

      const current = totals.get(symbol) ?? 0;
      totals.set(symbol, current + (trade.quantity ?? 0));
    }

    const aggregatedPositions: Position[] = Array.from(totals.entries()).map(([symbol, quantity]) => ({
      symbol,
      quantity,
      avgCost: 0,
      marketPrice: null,
      marketValue: null,
      unrealizedPnL: 0
    }));

    this.positions.set(aggregatedPositions);
  }

  addTrade() {
    this.error.set(null);
    const accountId = this.selectedAccountId();
    if (accountId == null) {
      this.error.set('Please select an account before adding a trade.');
      return;
    }

    const t: Trade = {
      ...this.newTrade,
      id: this.getNextTradeId(),
      symbol: this.newTrade.symbol.trim().toUpperCase(),
      accountId
    };

    const accounts = this.accounts();
    const targetIndex = accounts.findIndex(a => a.id === accountId);
    if (targetIndex === -1) {
      this.error.set('Selected account could not be found.');
      return;
    }

    const targetAccount = accounts[targetIndex];
    const updatedAccount: Account = {
      ...targetAccount,
      trades: [...(targetAccount.trades ?? []), t]
    };

    const updatedAccounts = [...accounts];
    updatedAccounts[targetIndex] = updatedAccount;
    this.accounts.set(updatedAccounts);
    this.onAccountChanged(accountId);

    this.newTrade = { symbol: 'AAPL', quantity: 10, openDate: new Date().toISOString().slice(0,10), openPrice: 100 };
  }

  removeTrade(id?: number) {
    if (id == null) {
      return;
    }

    const accountId = this.selectedAccountId();
    if (accountId == null) {
      return;
    }

    const accounts = this.accounts();
    const targetIndex = accounts.findIndex(a => a.id === accountId);
    if (targetIndex === -1) {
      return;
    }

    const targetAccount = accounts[targetIndex];
    const existingTrades = targetAccount.trades ?? [];
    const filteredTrades = existingTrades.filter(t => t.id !== id);
    if (filteredTrades.length === existingTrades.length) {
      return;
    }

    const updatedAccount: Account = {
      ...targetAccount,
      trades: filteredTrades
    };

    const updatedAccounts = [...accounts];
    updatedAccounts[targetIndex] = updatedAccount;
    this.accounts.set(updatedAccounts);
    this.onAccountChanged(accountId);
  }


  // private loadTradesForSelectedAccount() {
  //   const id = this.selectedAccountId();
  //   if (id == null) {
  //     this.trades.set([]);
  //     return;
  //   }

  //   const account = this.accounts().find(a => a.id === id);
  //   this.trades.set(account?.trades ? [...account.trades] : []);
  // }

  private getNextTradeId(): number {
    const accounts = this.accounts();
    let maxId = 0;
    for (const account of accounts) {
      for (const trade of account.trades ?? []) {
        if (typeof trade.id === 'number' && trade.id > maxId) {
          maxId = trade.id;
        }
      }
    }
    return maxId + 1;
  }
}
