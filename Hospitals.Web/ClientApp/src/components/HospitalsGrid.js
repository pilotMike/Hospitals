import 'react-data-grid/lib/styles.css'
import { AgGridReact } from 'ag-grid-react';
import { Component } from 'react'
import FetchHospitals from '../fetchHospitals';

import 'ag-grid-community/styles/ag-grid.css'; // Core grid CSS, always needed
import 'ag-grid-community/styles/ag-theme-alpine.css'; // Optional theme CSS
import AddHospital from './AddHospital';


export class HospitalsGrid extends Component {
    constructor() {
        super();
        this.state = { hospitals: [], selected: [] };
        this.onRowSelected = this.onRowSelected.bind(this);
        this.comcomponentDidMount = this.componentDidMount.bind(this);
        this.handleSubmit = this.handleSubmit.bind(this);
        this.deleteSelected = this.deleteSelected.bind(this);
    }

    /*
     * This class is pretty tightly coupled to a data source, so it's not usable for other data.
     * Normally I'd like to see behaviors implemented and the data source decoupled so that the
     * grid can be reused.
     * 
     * I went through a couple of different react data grids to find one that was both free
     * and reasonable enough to use here.
     * 
     * Also, there's no error handling, spinners, etc here.
     * 
     * */

    // this can also be in the parent so the grid can be reused with different data
     onRowsChange({rows, data}) {
        for (const i in data.indexes) {
            var updatedRow = rows[i];
            // no batching
            // also no PATCH, but it should be
            //await FetchHospitals.post(updatedRow);
            console.log(updatedRow);
        }
    }

    async componentDidMount() {
        const hospitals = await FetchHospitals.list();
        
        console.log(hospitals);
        this.state = {hospitals, selected: this.state.selected ?? []};
        this.setState(this.state);
    }

    async deleteSelected() {
        // hope nothing goes wrong!
        const selectedIds = this.state.selected;
        const remainingHospitals = this.state.hospitals.filter(h => !selectedIds.includes(h.id));
        if (selectedIds) {
            for (let id of selectedIds) {
                await FetchHospitals.delete(id);
            }
        }
        this.setState({ hospitals: remainingHospitals, selected: this.state.selected });
    }

    async onRowEditingStopped(event) {
        //console.log('editing stopped');
        //console.log(event);
        const hospital = event.data;
        FetchHospitals.put(hospital);
    }

    onRowSelected(event) {
        console.log(event);
        console.log(this.state);
        const selected = event.node.isSelected();
        const id = event.node.data.id;
        if (!this.state.selected) {
            this.state.selected = [];
        }
        if (selected) {
            this.state.selected.push(id);
        } else {
            const idx = this.state.selected.indexOf(id);
            if (idx) {
                this.state.selected.splice(idx, 1);
                this.setState(this.state);
            }
        }
        console.log(this.state);
    }

    async handleSubmit(hospitalName) {
        // error handling? never heard of her!
        var hospital = await FetchHospitals.post({ name: hospitalName });
        var hospitals = [...this.state.hospitals, hospital ];
        this.setState({hospitals, selected:this.state.selected});
    }

    render() {
        const {hospitals} = this.state;

        const columnDefs = [
            { field: 'id', checkboxSelection: true },
            { field: 'name', editable: true}
        ];
        const defaultColumnDef = {
            
        };

        return hospitals && (
            <>
                <div>
                <div style={{ height: '100%', boxSizing: 'border-box' }}>
                    <div style={{ height: '500px' }} className="ag-theme-alpine">

                        <div>
                            <AddHospital onSubmit={this.handleSubmit} />
                        </div>
                        <AgGridReact
                                rowData={hospitals}
                                columnDefs={columnDefs}
                                defaultColumnDef={defaultColumnDef}
                                rowSelection='multiple'
                                onRowEditingStopped={this.onRowEditingStopped}
                                stopEditingWhenCellsLoseFocus={true}
                                editType={'fullRow'}
                                onRowSelected={this.onRowSelected} />
                        </div>
                    </div>
                </div>
                <button type='button' style={{ marginTop: '50px' }}
                    onClick={this.deleteSelected} > Delete Selected</button>
            </>
        )
    }
}