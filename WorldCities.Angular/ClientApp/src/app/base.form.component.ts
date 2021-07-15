import { Component } from '@angular/core';
import { FormGroup } from '@angular/forms';

@Component({
  template: ''
})

export abstract class BaseFormComponent {
  //The form model
  form: FormGroup;

  constructor() { }

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
