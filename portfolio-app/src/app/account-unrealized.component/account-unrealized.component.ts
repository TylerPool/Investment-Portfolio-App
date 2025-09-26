import { NgFor, NgIf, DecimalPipe } from '@angular/common';
import { Component, EventEmitter, Input, Output } from '@angular/core';
import { Account, Position, Trade } from '../models';

@Component({
  selector: 'app-account-unrealized',
  standalone: true,
  imports: [NgIf, NgFor, DecimalPipe],
  templateUrl: './account-unrealized.component.html',
  styleUrls: ['./account-unrealized.component.css']
})
export class AccountUnrealizedComponent {
  private _account: Account | null = null;
  positions: Position[] = [];
  lotDetails: Trade[] = [];

  @Input()
  set account(value: Account | null) {
    this._account = value;
    this.positions = this.calculatePositions(value?.trades ?? []);
    this.lotDetails = [...(value?.trades ?? [])];
  }

  get account(): Account | null {
    return this._account;
  }

  @Output() removeTrade = new EventEmitter<number | undefined>();

  private calculatePositions(trades: Trade[]): Position[] {
    if (!trades.length) {
      return [];
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

    return Array.from(totals.entries()).map(([symbol, quantity]) => ({
      symbol,
      quantity,
      avgCost: 0,
      marketPrice: null,
      marketValue: null,
      unrealizedPnL: 0
    }));
  }
}
