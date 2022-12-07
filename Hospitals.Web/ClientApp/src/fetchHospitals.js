class FetchHospitals {
    // urls should be managed with axios and the CI/CD pipeline
    async list() {
        const response = await fetch('hospital');
        const data = await response.json();
        console.log(data);
        return data;
    }

    async fetch(id) {
        const response = await fetch('hospital/' + id);
        const data = await response.json();
        return data;
    }

    async post(hospital) {
        const requestOptions = {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(hospital)
        };
        var response = await fetch('hospital/', requestOptions);
        const data = await response.json();
        return data;
    }

    async put(hospital) {
        const requestOptions = {
            method: 'PUT',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(hospital)
        };
        console.log(requestOptions.body);
        var response = await fetch('hospital/' + hospital.id, requestOptions);
    }

    async delete(id) {
        await fetch('hospital/' + id, { method: 'DELETE' });
    }
}
export default FetchHospitals = new FetchHospitals();