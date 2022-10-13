import { ComponentFixture, TestBed } from '@angular/core/testing';

import { TestErorsComponent } from './test-erors.component';

describe('TestErorsComponent', () => {
  let component: TestErorsComponent;
  let fixture: ComponentFixture<TestErorsComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ TestErorsComponent ]
    })
    .compileComponents();

    fixture = TestBed.createComponent(TestErorsComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
