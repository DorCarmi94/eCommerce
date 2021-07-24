import React, {Component} from "react";
import "./Register.css"
import {StoreApi} from '../Api/StoreApi'
import {Redirect, withRouter} from "react-router-dom";

import {makeDiscountNodeLeaf} from "../Data/StorePolicies/DiscountInfoTree";
import AddRule from "./AddRule";

class Discount extends Component {
    static displayName = Discount.name;
    handleChange;

    constructor(props) {
        super(props)
        this.state = {
            discount:0,
            rule:undefined,
            submitted:false
        }
        this.storeApi = new StoreApi();

        this.handleInputChange = this.handleInputChange.bind(this);


    }

    componentDidUpdate(prevProps, prevState, snapshot){
        const {rule,discount} = this.state
        if(this.state !== prevState ) {
            this.props.addDiscount(makeDiscountNodeLeaf(rule,parseFloat(discount)),this.props.index)
        }
    }


     // createDiscountLeaf(){
     //    const {rule,discount} = this.state
     //        return makeDiscountNodeLeaf(rule,parseInt(discount));
     //
     //    }

        

    addRuleToDiscount (rule)  {
        this.setState({
            rule:rule
        })
    }

    handleInputChange(event){
        const target = event.target;
        this.setState({
            [target.name]: target.value
        });
    }
    render () {
        return (
                <div className="RegisterWindow">
                        <h3>{`Add Discount`}</h3>

                    <AddRule makeRule={(rule) => this.addRuleToDiscount(rule)} storeId ={this.props.storeId}/>
                        <div><label>Enter Discount</label>
                            <input type="number" name="discount" value={this.state.discount} onChange={this.handleInputChange}
                                    placeholder={'Enter Discount'} required/></div>
                </div>
        );
    }
}

export default withRouter(Discount);