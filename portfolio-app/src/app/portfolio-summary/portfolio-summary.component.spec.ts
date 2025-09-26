import { ComponentFixture, TestBed } from '@angular/core/testing';

import { PortfolioSummary as PortfolioSummary } from './portfolio-summary.component';

describe('PortfolioSummary', () => {
  let component: PortfolioSummary;
  let fixture: ComponentFixture<PortfolioSummary>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [PortfolioSummary]
    })
    .compileComponents();

    fixture = TestBed.createComponent(PortfolioSummary);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
