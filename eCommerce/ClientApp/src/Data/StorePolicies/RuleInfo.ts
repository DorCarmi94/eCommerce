import {Item} from "../Item";
import {Combinations} from "./Combinations";
import {Comperators} from "./Comperators";

export enum RuleType {
    Amount,
    Total_Amount,
    Category,
    Date,
    Default,
    IsItem,
    Age
}

export var RuleTypesOptions =["Amount","Total_Amount","Category","Date","Default","IsItem","Age"]

export interface RuleInfo {
    ruleType: RuleType,
    whatIsTheRuleFor: string,
    kind: string | null
    itemId: string,
    comperator: Comperators
}

export function makeRuleInfo(ruleType: RuleType, whatIsTheRuleFor: string,
                             itemId: string, comperator: Comperators): RuleInfo{
    return {
        ruleType: ruleType,
        whatIsTheRuleFor: whatIsTheRuleFor,
        kind: null,
        itemId: itemId,
        comperator: comperator
    }
}

// Rules tree

export enum RuleNodeType {
    Leaf,
    Composite,
}

export type RuleNode = RuleNodeLeaf | RuleNodeComposite

export type RuleNodeLeaf = {
    type: RuleNodeType,
    ruleInfo: RuleInfo 
}

export function makeRuleNodeLeaf(ruleInfo: RuleInfo): RuleNodeLeaf {
    return {
        type: RuleNodeType.Leaf,
        ruleInfo: ruleInfo
    }
}

export type RuleNodeComposite = {
    type: RuleNodeType,
    ruleA: RuleNode,
    ruleB: RuleNode,
    combination: Combinations
}

export function makeRuleNodeComposite(
    ruleA: RuleNode, ruleB: RuleNode, combination: Combinations): RuleNodeComposite {
    return {
        type: RuleNodeType.Composite,
        ruleA: ruleA,
        ruleB: ruleB,
        combination: combination
    }
}