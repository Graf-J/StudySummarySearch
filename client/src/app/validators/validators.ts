import { AbstractControl } from '@angular/forms';

function ValidateFileExtension(control: AbstractControl) {
    if (control.value) {
      const splitted = control.value.split('.');
      const extension = splitted[splitted.length - 1];
    
      if (!(extension === 'jpg')) {
        return { invalidFileExtension: true };
      }
    }
    return null;
}

export {
    ValidateFileExtension
}