import React, {Component} from "react";
import "./Register.css"
import {StoreApi} from '../Api/StoreApi'
import {Redirect, withRouter} from "react-router-dom";
import {
    makeRuleNodeComposite, makeRuleNodeLeaf
} from '../Data/StorePolicies/RuleInfo'
import AddRule from "./AddRule";
import {Combinations, CombinationsNames} from "../Data/StorePolicies/Combinations";
import {makeDiscountCompositeNode, makeDiscountNodeLeaf} from "../Data/StorePolicies/DiscountInfoTree";
import Discount from "./Discount";
import Rule from "./Rule";

class AddDiscount extends Component {
    static displayName = AddDiscount.name;
    handleChange;

    constructor(props) {
        super(props)
        this.state = {
            discount:0,
            selectedCombination:0,
            toggler:false,
            firstDiscount:undefined,
            secondDiscount:undefined,
            discounts:[undefined],
            selectedCombinations:[],
            submitted:false
        }
        this.storeApi = new StoreApi();

        this.handleSubmit = this.handleSubmit.bind(this);
        this.toggle = this.toggle.bind(this);
        this.toggleUp = this.toggleUp.bind(this);
        this.toggleDown = this.toggleDown.bind(this);
        this.handleInputChange = this.handleInputChange.bind(this);


    }
    //
    // redirectToHome = (path) => {
    //     const { history } = this.props;
    //     if(history) history.push(path);
    // }



    async handleSubmit(event){
        const {discounts,selectedCombinations} = this.state
        const {storeId} = this.props
        event.preventDefault();
        let currNode = undefined
        discounts.forEach((discount,index) =>{
            if(!currNode){
                currNode = discount
            }
            else{
                currNode = makeDiscountCompositeNode(currNode,discount,selectedCombinations[index-1])
            }
        })
        console.log('&&&&&&&&&&&&&&&&')
        console.log(storeId)
        console.log(currNode)
        console.log('&&&&&&&&&&&&&&&&')

        const res = await this.storeApi.addDiscountToStore(storeId, currNode)

        if(res && res.isSuccess) {
            alert('add discount succeed')
            this.setState({
                submitted:true
            })
        }
        else{
            if(res) {
                alert(`add discount failed because- ${res.error}`)
            }
        }
    }
    // async handleSubmit(event){
    //     const {firstDiscount,secondDiscount,selectedCombination,discount,toggler} = this.state
    //     const {storeId} = this.props
    //     event.preventDefault();
    //     if(firstDiscount) {
    //         let res = undefined
    //         let discount = firstDiscount
    //         if(toggler && secondDiscount){
    //             discount = makeDiscountCompositeNode(firstDiscount, secondDiscount, parseInt(selectedCombination));
    //         }
    //         res = await this.storeApi.addDiscountToStore(storeId, discount)
    //
    //         if(res && res.isSuccess) {
    //             alert('add policy succeed')
    //             this.setState({
    //                 submitted:true
    //             })
    //         }
    //         else{
    //             if(res) {
    //                 alert(`add policy failed because- ${res.error}`)
    //             }
    //         }
    //     }
    //
    //
    // }




    toggle(event){
        this.setState({
            toggler:!this.state.toggler
        })
    }

    addFirstDiscount(discount){
        this.setState({
            firstDiscount:discount
        })
    }

    addSecondDiscount(discount){
        this.setState({
            secondDiscount:discount
        })
    }



    handleInputChange(event){
        const target = event.target;
        this.setState({
            [target.name]: target.value
        });
    }

    toggleDown(event){
        const {discounts,selectedCombinations} = this.state
        let tempDiscounts = discounts
        let tempSelectedCombinations = selectedCombinations
        tempDiscounts.pop()
        tempSelectedCombinations.pop()
        this.setState({
            rules:tempDiscounts,
            toggler:!this.state.toggler,
            selectedCombinations:tempSelectedCombinations
        })
    }


    toggleUp(event){
        const {discounts,selectedCombinations} = this.state
        let tempDiscounts = discounts
        let tempSelectedCombinations = selectedCombinations
        tempDiscounts.push(undefined)
        tempSelectedCombinations.push(0)
        this.setState({
            rules:tempDiscounts,
            selectedCombinations:tempSelectedCombinations
        })
    }

    chooseCombination(event,index){
        const {selectedCombinations} = this.state
        let tempSelectedCombinations = selectedCombinations
        tempSelectedCombinations[index-1] = parseInt(event.target.value)
        this.setState({
            selectedCombinations:tempSelectedCombinations
        })
    }

    addNewDiscount(discount,index){
        const {discounts} = this.state
        let tempDiscounts = discounts
        tempDiscounts[index] =  discount
        this.setState({
            discounts :tempDiscounts
        })}
    
    render () {
        const {discounts,submitted} = this.state
        const {storeId} = this.props
        const combinatorValue = CombinationsNames[this.state.selectedCombination]
        if(submitted){
            return <Redirect exact to={`/store/${storeId}`}/>
        }
        else{
            return (
                // <main className="RegisterMain">
                <div className="RegisterWindow">
                    <h3>{`Add Discounts To The Policy Of The Store: ${storeId}`}</h3>
                    <form  onSubmit={this.handleSubmit}>
                        {
                            discounts.map((rule,index) =>{
                                return  (
                                    <div>
                                        {discounts.length > 1 && index > 0?
                                            <label>
                                                Choose Combination:
                                                <select  onChange={(event) => this.chooseCombination(event,index)} name="selectedCombination" className="searchContainer">
                                                    {CombinationsNames.map((combination,index) => <option  value={index}>{combination}</option>)}
                                                </select>
                                            </label> : null}
                                        <Discount addDiscount={(rule,index) => this.addNewDiscount(rule,index)} index={index} storeId = {storeId}/>
                                    </div>
                                )
                            })
                        }
                        {discounts.length > 1 ? <div><button onClick={this.toggleDown}>Don't Combine Another Rule</button></div> : null}
                        <button onClick={this.toggleUp}>{`Combine Another Discount`}</button>
                        
                        {/*<Discount addDiscount={(discount) =>this.addFirstDiscount(discount)} storeId={storeId}/>*/}
                        {/*<button onClick={this.toggle}>{`${toggler? "Don't " : ''}Combine Another Discount`}</button>*/}
                        {/*{*/}
                        {/*    toggler ?*/}
                        {/*        <>*/}
                        {/*            <div>*/}
                        {/*                <label>*/}
                        {/*                    Choose Combination:*/}
                        
                        {/*                    <select  onChange={this.handleInputChange} name="selectedCombination" className="searchContainer">*/}
                        {/*                        {CombinationsNames.map((combination,index) => <option  value={index}>{combination}</option>)}*/}
                        {/*                    </select>*/}
                        {/*                </label>*/}
                        {/*            </div>*/}
                        {/*            <Discount addDiscount={(discount) =>this.addSecondDiscount(discount)} storeId={storeId}/></>:*/}
                        {/*        null*/}
                        {/*}*/}
                        <div className="CenterItemContainer">
                            <input className="action" type="submit" value="Add Policy"/>
                        </div>
                    </form>
                </div>
                // </main>
            );
        }
    }}

export default withRouter(AddDiscount);