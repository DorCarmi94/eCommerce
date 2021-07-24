import React, {Component} from "react";
import "./Register.css"
import {StoreApi} from '../Api/StoreApi'
import {withRouter} from "react-router-dom";
import {RuleType,RuleTypesOptions,makeRuleNodeLeaf,makeRuleInfo} from '../Data/StorePolicies/RuleInfo'
import {Item} from '../Data/Item'
import {Comperators,ComperatorsNames} from "../Data/StorePolicies/Comperators";

class Rule extends Component {
    static displayName = Rule.name;
    handleChange;

    constructor(props) {
        super(props)
        this.state = {
            kind:'',
            ruleType:0,
            whatIsTheRuleOf:'',
            selectedComperator:0,
            selectedItem:'',
            items:[],
        }
        this.storeApi = new StoreApi();

        // this.handleSubmit = this.handleSubmit.bind(this);
        this.handleInputChange = this.handleInputChange.bind(this);
    }

    redirectToHome = (path) => {
        const { history } = this.props;
        if(history) history.push(path);
    }

    async componentDidMount() {
        const fetchedItems = await this.storeApi.getAllItems(this.props.storeId)
        if (fetchedItems && fetchedItems.isSuccess) {
            console.log(fetchedItems)
            
            this.setState({
                items: fetchedItems.value,
                selectedItem:fetchedItems.value.length > 0 ? fetchedItems.value[0].itemName : ''
            })
        }
        else{
            alert("fetch item failed")
        }

    }
    
    componentDidUpdate(prevProps, prevState, snapshot){
        const {ruleType,whatIsTheRuleOf,selectedComperator,selectedItem} = this.state
        if(this.state !== prevState && this.props.addRule) {
            // alert(ruleType)
            this.props.addRule((makeRuleInfo(parseInt(ruleType), whatIsTheRuleOf
                , selectedItem, parseInt(selectedComperator))),this.props.index)
        }
    }

    // async handleSubmit(event){
    //     const {storeId} = this.props
    //     const {ruleType,whatIsTheRuleOf,selectedItem,selectedComperator} = this.state
    //     // const ruleTypeIdx=ruleType
    //     // const comperatorIdx = selectedComperator
    //     // console.log(ruleTypeIdx)
    //     // console.log(comperatorIdx)
    //     // console.log('&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&&')
    //     event.preventDefault();
    //     const res = await this.storeApi.addRuleToStorePolicy(storeId,makeRuleNodeLeaf(makeRuleInfo(parseInt(ruleType),whatIsTheRuleOf,
    //         selectedItem,parseInt(selectedComperator))))
    //
    //     if(res && res.isSuccess) {
    //         alert('add rule succeed')
    //         this.redirectToHome(`/store/${storeId}`)
    //     }
    //     else{
    //         if(res) {
    //             alert(`add rule failed because- ${res.error}`)
    //         }
    //     }
    //     // alert("Rule Has Been Added")
    // }


    
    handleInputChange(event){
        const {ruleType,whatIsTheRuleOf,selectedComperator,selectedItem} = this.state
        const target = event.target;
        console.log(target.name)
        this.setState({
            [target.name]: target.value
        });


    }

    render () {
        const {items} = this.state
        const comperatorValue = ComperatorsNames[this.state.selectedComperator]
        const ruleTypeValue = RuleTypesOptions[this.state.ruleType]
        // console.log(ruleTypeValue)
        // console.log(comperatorValue)
        return (
            // <main className="RegisterMain">
                <div className="RegisterWindow">
                    <div className="CenterItemContainer">
                        <h3>{`Add Rule`}</h3>
                    </div>
                    {/*<form  onSubmit={this.handleSubmit}>*/}
                        <div><label>
                            Choose An Item:
                            <select  onChange={this.handleInputChange} name="selectedItem" className="searchContainer">
                                {items.map((item) => <option  value={item.itemName}>{item.itemName}</option>)}
                            </select>
                        </label></div>
                        <label>
                            Choose Rule Type:
                            <select  onChange={this.handleInputChange} name="ruleType" className="searchContainer">
                                {RuleTypesOptions.map((ruleType,index) => <option  value={index}>{ruleType}</option>)}
                            </select>
                        </label>
                        <div><input type="text" name="whatIsTheRuleOf" value={this.state.whatIsTheRuleOf} onChange={this.handleInputChange}
                               placeholder={'What Is The Rule Of'} required/></div>
                        <div><input type="text" name="kind" value={this.state.kind} onChange={this.handleInputChange}
                                    placeholder={'Enter Kind Of Rule'} required/></div>
                        <div><label>
                            Choose Comperator:
                            <select  onChange={this.handleInputChange} name="selectedComperator" className="searchContainer">
                                {ComperatorsNames.map((comperator,index) => <option  value={index}>{comperator}</option>)}
                            </select>
                        </label></div>
                    {/*{*/}
                    {/*//     !this.props.addRule?*/}
                    {/*//         <div className="CenterItemContainer">*/}
                    {/*//             <input className="action" type="submit" value="Add Rule"/>*/}
                    {/*//         </div>*/}
                    {/*//      : null*/}
                    {/*// }*/}
                    {/*// </form>*/}
            </div>
            // </main>
        );
    }
}

export default withRouter(Rule);