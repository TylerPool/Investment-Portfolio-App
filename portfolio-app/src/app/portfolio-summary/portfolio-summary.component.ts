import { Component, EventEmitter, Input, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Account, Portfolio } from '../models';

@Component({
  selector: 'app-portfolio-summary',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './portfolio-summary.component.html',
  styleUrls: ['./portfolio-summary.component.css']
})
export class PortfolioSummary {
  @Input() portfolio: Portfolio | null = null;
  @Input() selectedAccountId: string | null = null;
  @Output() selectedAccountIdChange = new EventEmitter<string | null>();

  get accounts(): Account[] {
    return this.portfolio?.accounts ?? [];
  }

  onAccountChange(val: string | number | null) {
    if (val === '' || val == null) {
      this.selectedAccountIdChange.emit(null);
      return;
    }

    this.selectedAccountIdChange.emit(String(val));
  }
}
