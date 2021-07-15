import { Component, Inject, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormGroup, FormControl, Validators, AbstractControl, AsyncValidatorFn } from '@angular/forms';
import { Observable, Subject, Subscription } from 'rxjs';
import { map, takeUntil } from 'rxjs/operators';

import { City } from './City';
import { Country } from './../countries/Country';
import { BaseFormComponent } from '../base.form.component';
import { CityService } from './city.service';
import { ApiResult } from '../base.service';

@Component({
  selector: 'app-city-edit',
  templateUrl: './city-edit.component.html',
  styleUrls: ['./city-edit.component.css']
})

export class CityEditComponent extends BaseFormComponent implements OnInit {
  //The view title
  title: string;

  //The form model
  form: FormGroup;

  //The city object to edit or create
  city: City;

   //The city object id, as fetched from the active route:
  //It's NULL when we're adding a new city, and not NULL when we're editing an existing one.
  id?: number;

  //The countries array for the select
  countries: Country[];


  //Activity Log (for debugging purposes)
  activityLog: string = '';

  //private subscriptions: Subscription = new Subscription();

  //Notifier subject (to avoid memory leaks)
  private destroySubject: Subject<boolean> = new Subject<boolean>();

  constructor(
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private cityService: CityService,
    @Inject('BASE_URL') private baseUrl: string)
  {
    super();
  }

  ngOnInit() {
    this.form = new FormGroup({
      name: new FormControl('', Validators.required),

      lat: new FormControl('', [
        Validators.required,
        Validators.pattern(/^[-]?[0-9]+(\.[0-9]{1,4})?$/)
      ]),

      lon: new FormControl('', [
        Validators.required,
        Validators.pattern(/^[-]?[0-9]+(\.[0-9]{1,4})?$/)
      ]),

      //lat: new FormControl('', Validators.required),
      //lon: new FormControl('', Validators.required),
      countryId: new FormControl('', Validators.required)
    }, null, this.isDupeCity());

    //React to form changes
    this.form.valueChanges
      .pipe(takeUntil(this.destroySubject))
      .subscribe(() =>
      {
        if (!this.form.dirty)
        {
          this.log("Form Model has been loaded.");
        }
        else {
          this.log("Form was updated by the user.");
        }
      });


    //React to changes in the form.name control
    this.form.get("name")!.valueChanges
      .pipe(takeUntil(this.destroySubject))
      .subscribe(() =>
      {
        if (!this.form.dirty)
        {
          this.log("Name has been loaded with initial values.");
        }
        else
        {
          this.log("Name was updated by the user.");
        }
      });

    this.loadData();
  }

  ngOnDestroy()
  {
    //Emit a value with the takeUntil notifier
    this.destroySubject.next(true);

    //Unsubscribe from the notifier itself
    this.destroySubject.unsubscribe();
  }

  log(str: string) {
    console.log("["
      + new Date().toLocaleString()
      + "] " + str);
  }

  //log(str: string)
  //{
  //  this.activityLog += "["
  //    + new Date().toLocaleString()
  //    + "] " + str + "<br />";
  //}

  loadData() {
    //Load countries
    this.loadCountries();

    // retrieve the ID from the 'id'
    this.id = +this.activatedRoute.snapshot.paramMap.get('id');

    if (this.id) {
      //Update a city
      //Fetch the city from the server
      this.cityService.get<City>(this.id).subscribe(result => {
        this.city = result;
        this.title = "Edit - " + this.city.name;

        // update the form with the city value
        this.form.patchValue(this.city);

      }, error => console.error(error));
    }
    else {
      //Add a city
      this.title = "Create a new City";
    }
  }

  loadCountries() {
    //Fetch all the countries from the server
    this.cityService.getCountries<ApiResult<Country>>(0, 9999, "name", null, null, null)
      .subscribe(result =>
    {
      this.countries = result.data;
    }, error => console.error(error));
  }

  onSubmit() {
    var city = (this.id) ? this.city : <City>{};

    city.name = this.form.get("name").value;
    city.lat = +this.form.get("lat").value;
    city.lon = +this.form.get("lon").value;
    city.countryId = +this.form.get("countryId").value;

    if (this.id) {
      //Edit a city
      this.cityService
        .put<City>(city)
        .subscribe(result => {
          console.log("City " + city.id + " has been updated.");

          // go back to cities view
          this.router.navigate(['/cities']);

        }, error => console.error(error));
    }
    else {
      //Add a city
      this.cityService
        .post<City>(city)
        .subscribe(result => {
          console.log("City " + result.id + " has been created.");

          // go back to cities view
          this.router.navigate(['/cities']);

        }, error => console.error(error));
    }
  }

  isDupeCity(): AsyncValidatorFn {
    return (control: AbstractControl): Observable<{ [key: string]: any } | null> => {
      var city = <City>{};

      city.id = (this.id) ? this.id : 0;
      city.name = this.form.get("name").value;
      city.lat = +this.form.get("lat").value;
      city.lon = +this.form.get("lon").value;
      city.countryId = +this.form.get("countryId").value;

      return this.cityService.isDupeCity(city).pipe(map(result =>
      {
        return (result ? { isDupeCity: true } : null);
      }));
    }
  }
}
