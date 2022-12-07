import React, { Component } from 'react';
import { HospitalsGrid } from './HospitalsGrid';
import FetchHospitals from '../fetchHospitals';

export class Home extends Component {
  static displayName = Home.name;

  render() {

    return (
      <div>
        <h1>Hospitals</h1>
        <HospitalsGrid  />
      </div>
    );
  }
}
