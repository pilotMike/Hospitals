import { useState } from 'react'
import './AddHospital.css';

export default function AddHospital({ onSubmit }) {
    const [text, setText] = useState('');

    const handleChange = (event) => {
        setText(event.target.value);
    }

    const handleClick = () => {
        onSubmit(text);
    }

    return (
        <div className='container'>
            <span>Add new Hospital</span>
            <div style={{ display: 'inline-block' }}>
                <input
                    type='text'
                    maxLength='100'
                    onChange={handleChange} />
                <button onClick={handleClick}>Save</button>
            </div>
        </div>
    );
}