import { ComponentFixture, TestBed } from '@angular/core/testing';

import { AccountUnrealizedComponent } from './account-unrealized.component';

describe('AccountUnrealizedComponent', () => {
  let component: AccountUnrealizedComponent;
  let fixture: ComponentFixture<AccountUnrealizedComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [AccountUnrealizedComponent]
    })
    .compileComponents();

    fixture = TestBed.createComponent(AccountUnrealizedComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
