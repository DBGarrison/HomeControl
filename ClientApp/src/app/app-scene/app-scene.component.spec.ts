import { async, ComponentFixture, TestBed } from '@angular/core/testing';

import { AppSceneComponent } from './app-scene.component';

describe('AppSceneComponent', () => {
  let component: AppSceneComponent;
  let fixture: ComponentFixture<AppSceneComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ AppSceneComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(AppSceneComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
