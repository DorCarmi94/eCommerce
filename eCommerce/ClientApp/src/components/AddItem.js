import React, {Component} from "react";
import {Form,Button} from 'react-bootstrap'
import "./Register.css"
import {StoreApi} from '../Api/StoreApi'
import {withRouter} from "react-router-dom";
import {Item} from "../Data/Item";

class AddItem extends Component {
    static displayName = AddItem.name;

    constructor(props) {
        super(props)
        this.state = {
            itemName:'',
            amount:undefined,
            storeId:'',
            category:'',
            keyWords:'',
            price:undefined
        }

        this.handleSubmit = this.handleSubmit.bind(this);
        this.handleInputChange = this.handleInputChange.bind(this);
    }

    redirectToHome = (path) => {
        const { history } = this.props;
        if(history) history.push(path);
    }

    async handleSubmit(event){
        const {itemName,amount,category,keyWords,price} = this.state
        const {storeId} = this.props
        event.preventDefault();
        const res = await StoreApi.addItem(Item.createItem(itemName, storeId, amount, category, [keyWords], price))
        if(res && res.isSuccess) {
            alert('add item succeed')
            this.redirectToHome(`/store/${storeId}`)
        }
        else{
            alert(`add item failed because- ${res.error}`)
        }

    }
    handleInputChange(event){
        const target = event.target;
        this.setState({
            [target.name]: target.value
        });
    }


    render () {
        return (
            <main className="RegisterMain">
                <div className="RegisterWindow">
                    <div className="CenterItemContainer">
                        <h3>Add an Item</h3>
                    </div>
                    <form className="RegisterForm" onSubmit={this.handleSubmit}>
                        <input type="text" name="itemName" value={this.state.itemName}
                               placeholder={'Enter Item Name'} onChange={this.handleInputChange} required/>
                        <input type="number" name="amount" value={this.state.amount} onChange={this.handleInputChange}
                               placeholder={'Enter amount'} required/>
                        <input type="text" name="category" value={this.state.category} onChange={this.handleInputChange}
                               placeholder={'Enter Item Category'} required/>
                        <input type="text" name="keyWords" value={this.state.keyWords} onChange={this.handleInputChange}
                               placeholder={'Enter Item keyWords'} required/>
                        <input type="number" name="price" value={this.state.price} onChange={this.handleInputChange}
                               placeholder={'Enter Item price'} required/>
                        <div className="CenterItemContainer">
                            <input className="action" type="submit" value="submit"/>
                        </div>
                    </form>
                </div>
            </main>
        );
    }
}

export default withRouter(AddItem);