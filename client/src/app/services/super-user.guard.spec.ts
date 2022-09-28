import { TestBed } from '@angular/core/testing';

import { SuperUserGuard } from './super-user.guard';

describe('SuperUserGuard', () => {
  let guard: SuperUserGuard;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    guard = TestBed.inject(SuperUserGuard);
  });

  it('should be created', () => {
    expect(guard).toBeTruthy();
  });
});
