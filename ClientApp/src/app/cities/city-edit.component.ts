import { Component, Inject, OnInit } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { ActivatedRoute, Router } from '@angular/router';
import { FormGroup, FormControl, Validators, AbstractControl, AsyncValidatorFn } from '@angular/forms';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';

import { City } from './City';
import { Country } from './../countries/Country';
import { BaseFormComponent } from '../base.form.component';

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

  constructor(
    private activatedRoute: ActivatedRoute,
    private router: Router,
    private http: HttpClient,
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
    this.loadData();
  }

  isDupeCity(): AsyncValidatorFn {
    return (control: AbstractControl): Observable<{ [key: string]: any } | null> => {
      var city = <City>{};
      city.id = (this.id) ? this.id : 0;
      city.name = this.form.get("name").value;
      city.lat = +this.form.get("lat").value;
      city.lon = +this.form.get("lon").value;
      city.countryId = +this.form.get("countryId").value;
      var url = this.baseUrl + "api/Cities/IsDupeCity";
      return this.http.post<boolean>(url, city).pipe(map(result => {
        return (result ? { isDupeCity: true } : null);
      }));
    }
  }

  loadData() {
    //Load countries
    this.loadCountries();

    // retrieve the ID from the 'id'
    this.id = +this.activatedRoute.snapshot.paramMap.get('id');

    if (this.id) {
      //Update a city
      //Fetch the city from the server
      var url = this.baseUrl + "api/Cities/" + this.id;

      this.http.get<City>(url).subscribe(result => {
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
    var url = this.baseUrl + "api/Countries";

    var params = new HttpParams()
      .set("pageIndex", "0")
      .set("pageSize", "9999")
      .set("sortColumn", "name");

    this.http.get<any>(url, { params }).subscribe(result => {
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
      var url = this.baseUrl + "api/Cities/" + this.city.id;

      this.http
        .put<City>(url, city)
        .subscribe(result => {
          console.log("City " + city.id + " has been updated.");

          // go back to cities view
          this.router.navigate(['/cities']);

        }, error => console.error(error));
    }
    else {
      //Add a city
      var url = this.baseUrl + "api/Cities";

      this.http
        .post<City>(url, city)
        .subscribe(result => {
          console.log("City " + result.id + " has been created.");

          // go back to cities view
          this.router.navigate(['/cities']);

        }, error => console.error(error));
    }
  }

  //Retrieve a FormControl
  getControl(name: string) {
    return this.form.get(name);
  }

  //Returns TRUE if the FormControl is valid
  isValid(name: string) {
    var e = this.getControl(name);
    return e && e.valid;
  }

  //Returns TRUE if the FormControl has been changed
  isChanged(name: string) {
    var e = this.getControl(name);
    return e && (e.dirty || e.touched);
  }

  //Returns TRUE if the FormControl is raising an error,
  //i.e. an invalid state after user changes
  hasError(name: string) {
    var e = this.getControl(name);
    return e && (e.dirty || e.touched) && e.invalid;
  }

}