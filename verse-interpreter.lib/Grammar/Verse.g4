﻿parser grammar Verse;
options {tokenVocab=VerseLexer;}

verse_text: ( program ) * EOF;

declaration : ID ':' type
            | ID ':=' (value_definition | constructor_body)
            | ID '=' (value_definition | constructor_body)
            | ID ':=' array_literal
            | ID '=' array_literal
            ;

value_definition : (INT | expression | constructor_body | string_rule | choice_rule 
                   | array_literal | array_index)
                 ;
                 

program : function_definition program
        | declaration program
        | function_definition program
        | function_call program
        | type_header program
        | type_member_definition program
        | expression program
        | (NEWLINE | NEWLINE NEWLINE) program
        | program ';' program
        | declaration
        | function_call
        | type_header
        | type_member_definition
        | function_definition
        | expression
        | array_index
        ;

block : declaration
      | function_call
      | expression
      | declaration
      | function_call
      | if_block
      ;

body : inline_body
              | NEWLINE spaced_body
              ;
              
inline_body : block ';' inline_body
            | block
            ;
            
spaced_body : INDENT block
            | INDENT block NEWLINE spaced_body
            ;


// Arrays/Tuples
array_literal : '(' array_elements ')'
              | '('')'
              ;


array_elements : value_definition (',' array_elements)*
               | declaration (',' array_elements)*
               ;

array_index : ID '[' INT ']'
            ;


// Functions

function_call : ID '(' param_call_item ')'
              | ID '(' ')' 
              ;

function_definition : ID function_param ':' type ':=' body
                    | ID function_param ':' type ':=' '{' body NEWLINE?'}'
                    ;
                     
function_param : '(' ')'
               | '(' param_def_item ')'
               ;
               
param_def_item     : declaration
                   | declaration ',' param_def_item
                   ;
                   
param_call_item : (INT | ID | function_call | expression)
                | (INT | ID | function_call | expression) ',' param_call_item
                ;

// Type definition

constructor_body : INSTANCE ID '('')'
                 ;

type_header : DATA ID '=' ID NEWLINE '{' type_body NEWLINE '}'
            ;
            
type_body : NEWLINE INDENT declaration
          | NEWLINE INDENT declaration type_body
          ;
   
type_member_definition : type_member_access '=' value_definition
                       ;

          
type_member_access : type_member_access '.' ID
                   | ID'.'ID
                   ;

// Strings
string_rule : SEARCH_TYPE
            ;

// Choice
choice_value : (INT | ID ) ;

choice_rule : choice_value '|' choice_value
            | choice_value '|' choice_rule
            | '('choice_rule ')'
            ;

            
// Conditionals

if_block    : 'if' '(' comp_expression ')' 'then' body 'else' body ;

comp_expression
    : comp_term
    | comp_expression comparsion_op comp_term 
    ;

comp_term
    : comp_factor
    | comp_term comparsion_op factor
    ;

comp_factor
    : comp_primary
    | comparsion_op comp_factor
    ;

comp_primary
    : ID
    | INT
    | '(' comp_expression ')'
    ;


// Math expression rules
expression
    : binary_expression
    ;

binary_expression
    : term operator term
    | binary_expression operator term
    ;

term
    : factor
    | term operator factor
    ;

factor
    : primary
    | operator factor
    ;

primary
    : ID
    | INT
    | type_member_access
    | function_call
    | '(' expression ')'
    ;


type : (INTTYPE | STRINGTYPE | ID ) ;
comparsion_op : ('>' | '<' | '|' | '=' )   ; 
operator : ('*' | '/' |'-'|'+'| '>');
