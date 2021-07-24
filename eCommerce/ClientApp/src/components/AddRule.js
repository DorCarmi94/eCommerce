import React, {Component} from "react";
import "./Register.css"
import {StoreApi} from '../Api/StoreApi'
import {Redirect, withRouter} from "react-router-dom";
import {
    makeRuleInfo,
    makeRuleNodeComposite, makeRuleNodeLeaf
} from '../Data/StorePolicies/RuleInfo'
import {CombinationsNames} from "../Data/StorePolicies/Combinations";
import {makeDiscountNodeLeaf} from "../Data/StorePolicies/DiscountInfoTree";
import Rule from "./Rule";

class AddRule extends Component {
    static displayName = AddRule.name;
    handleChange;

    constructor(props) {
        super(props)
        this.state = {
            selectedCombination:0,
            toggler:false,
            rules:[undefined],
            selectedCombinations:[],
            firstRule:undefined,
            secondRule:undefined,
            submitted:false
        }
        this.storeApi = new StoreApi();

        this.handleSubmit = this.handleSubmit.bind(this);
        this.toggleUp = this.toggleUp.bind(this);
        this.toggleDown = this.toggleDown.bind(this);
        this.handleInputChange = this.handleInputChange.bind(this);


    }

    componentDidUpdate(prevProps, prevState, snapshot){
        if(this.state !== prevState && this.props.makeRule) {
            this.props.makeRule(this.combineRule())
        }
    }
    
    combineRule() {
        const {rules,selectedCombinations} = this.state
        let currNode = undefined
        rules.forEach((rule,index) =>{
            if(!currNode){
                currNode = rule
            }
            else{
                currNode = makeRuleNodeComposite(currNode,rule,parseInt(selectedCombinations[index-1]))
            }
        })
        return currNode
    }

    async handleSubmit(event){
        const {storeId} = this.props
        event.preventDefault();
        const currNode = this.combineRule()
        const res = await this.storeApi.addRuleToStorePolicy(storeId, currNode)

        if(res && res.isSuccess) {
            alert('add rule succeed')
            this.setState({
                submitted:true
            })
        }
        else{
            if(res) {
                alert(`add rule failed because- ${res.error}`)
            }
        }
    }

    //
    // redirectToHome = (path) => {
    //     const { history } = this.props;
    //     if(history) history.push(path);
    // }





    toggleDown(event){
        const {rules,selectedCombinations} = this.state
        let tempRules = rules
        let tempSelectedCombinations = selectedCombinations
        tempRules.pop()
        tempSelectedCombinations.pop()
        this.setState({
            rules:tempRules,
            toggler:!this.state.toggler,
            selectedCombinations:tempSelectedCombinations
        })
    }


    toggleUp(event){
        const {rules,selectedCombinations} = this.state
        let tempRules = rules
        let tempSelectedCombinations = selectedCombinations
        tempRules.push(undefined)
        tempSelectedCombinations.push(0)
        this.setState({
            rules:tempRules,
            selectedCombinations:tempSelectedCombinations
        })
    }

    addNewRule(rule,index){
        const {rules} = this.state
        let tempRules = rules
        tempRules[index] =  makeRuleNodeLeaf(rule)
        this.setState({
            rules :tempRules
        })}



    chooseCombination(event,index){
        const {selectedCombinations} = this.state
        let tempSelectedCombinations = selectedCombinations
        tempSelectedCombinations[index-1] = event.target.value
        this.setState({
            selectedCombinations:tempSelectedCombinations
        })
    }


    handleInputChange(event){
        const target = event.target;
        this.setState({
            [target.name]: target.value
        });
    }
    
    showRules() {
        const {toggler,submitted,rules} = this.state
        const {storeId} = this.props


        return (
            <>
                {
                    rules.map((rule,index) =>{
                        return  (
                            <div>
                                {rules.length > 1 && index > 0?
                                    <label>
                                        Choose Combination:
                                        <select  onChange={(event) => this.chooseCombination(event,index)} name="selectedCombination" className="searchContainer">
                                            {CombinationsNames.map((combination,index) => <option  value={index}>{combination}</option>)}
                                        </select>
                                    </label> : null}
                                <Rule addRule={(rule,index) => this.addNewRule(rule,index)} index={index} storeId = {storeId}/>
                            </div>
                        )
                    })
                }
                {rules.length > 1 ? <div><button onClick={this.toggleDown}>Don't Combine Another Rule</button></div> : null}
                <button onClick={this.toggleUp}>{`Combine Another Rule`}</button>
                { !this.props.makeRule ?
                    <div className="CenterItemContainer">
                        <input className="action" type="submit" value="Add Rule To Policy"/>
                    </div>
                    : null }
            </>
        )
    }
    render () {
        const {toggler,submitted,rules} = this.state
        const {storeId} = this.props
        if(submitted){
            return <Redirect exact to={`/store/${storeId}`}/>
        }
        else{
            return (
                // <main className="RegisterMain">
                <div className="RegisterWindow">
                    <h3>{`Add Rule To Store Policy`}</h3>
                    { !this.props.makeRule ? <form  onSubmit={this.handleSubmit}>
                        {this.showRules()}
                    </form>
                        : 
                        this.showRules()}
                </div>
                // </main>
            );
        }
    }}

export default withRouter(AddRule);