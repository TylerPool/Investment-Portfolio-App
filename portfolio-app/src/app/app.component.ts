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
        const accountsWithTrades = accts.map(account => ({
          ...account,
          trades: account.trades ?? []
        }));

        this.accounts.set(accountsWithTrades);
        // default to first account if none selected yet
        if (accountsWithTrades.length && this.selectedAccountId() == null) {
          this.selectedAccountId.set(accountsWithTrades[0].id);
        }

        // Synchronize trades with the selected account
        this.loadTradesForSelectedAccount();
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
      error: e => this.error.set(e.message),
      complete: () => this.loading.set(false)
    });

    this.loadTradesForSelectedAccount();
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
    this.loadTradesForSelectedAccount();

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
    this.loadTradesForSelectedAccount();
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
        // no-op for now; could show toast/snackbar later
      },
      error: (e) => this.error.set(e.message)
    });
  }

  private loadTradesForSelectedAccount() {
    const id = this.selectedAccountId();
    if (id == null) {
      this.trades.set([]);
      return;
    }

    const account = this.accounts().find(a => a.id === id);
    this.trades.set(account?.trades ? [...account.trades] : []);
  }

  private loadPortfolioById(id: string){
    this.svc.getPortfolio().subscribe({
      next: (p) => this.portfolio.set(p),
      error: (e) => this.error.set(e.message)
    });
  }

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
