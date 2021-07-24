import React, {ChangeEvent, Component} from 'react';
import "./SearchComponent.css";
import {Link} from "react-router-dom";
import {NavLink} from "reactstrap";


export class SearchComponent extends Component {
    static displayName = SearchComponent.name;
    
    constructor(props) {
        super(props);
        this.state = {
            searchQuery: "",
            searchBy : "Item"
        }
        
        this.handleInputChange = this.handleInputChange.bind(this);
        this.changeFunc = this.changeFunc.bind(this);



    }

    handleInputChange(event){
        const value = event.target.value;
        this.setState({
            searchQuery: value
        });
        }


    

    changeFunc(e) {
        this.setState({
            searchBy:(e.target.value)
        })

    }

        render(){
        return (
            <div id = "selectBox" className="searchContainer">
                <select className="searchOptions" onChange={(e) => this.changeFunc(e)}>
                    <option value="Item">Item</option>
                    <option value="Store">Store</option>

                </select>
                <input placeholder="Search" value={this.state.searchQuery} onChange={this.handleInputChange}/>
                {this.state.searchBy ==='Item' ? <Link className="searchLink" exact to={`/searchItems/${this.state.searchBy}/${this.state.searchQuery}`}>
                        <div className="imageDiv">
                            <img src="/Images/search.png" alt="Search" className="imageFitSize"/>
                        </div>
                    </Link>
                    : <Link className="searchLink" exact to={`/store/${this.state.searchQuery}`}>
                        <div className="imageDiv">
                            <img src="/Images/search.png" alt="Search" className="imageFitSize"/>
                        </div>
                    </Link>}

            </div>
        )
    }
}
